import {useEffect, useRef} from "react";
import Map from "ol/Map";
import View from "ol/View";
import Feature from "ol/Feature";
import Point from "ol/geom/Point";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import OSM from "ol/source/OSM";
import VectorSource from "ol/source/Vector";
import {fromLonLat} from "ol/proj";
import {Circle, Fill, Stroke, Style} from "ol/style";
import type {Delivery, DeliveryRoute} from "../../types/delivery";
import {LineString} from "ol/geom";
import {defaults as defaultControls} from "ol/control/defaults";

type DeliveryMapProps = {
    deliveries: Delivery[];
    selectedRoute: DeliveryRoute | null;
};

export function DeliveryMap({deliveries, selectedRoute}: DeliveryMapProps) {
    const mapElementRef = useRef<HTMLDivElement | null>(null);
    const mapRef = useRef<Map | null>(null);
    const routeSourceRef = useRef<VectorSource | null>(null);
    const markerSourceRef = useRef<VectorSource | null>(null);

    useEffect(() => {
        if (!mapElementRef.current || mapRef.current) {
            return;
        }

        const markerSource = new VectorSource();
        markerSourceRef.current = markerSource;

        const markerLayer = new VectorLayer({
            source: markerSource,
            style: new Style({
                image: new Circle({
                    radius: 7,
                    fill: new Fill({color: "#22c55e"}),
                    stroke: new Stroke({
                        color: "#ffffff",
                        width: 2,
                    }),
                }),
            }),
        });

        const routeSource = new VectorSource();
        routeSourceRef.current = routeSource;

        const routeLayer = new VectorLayer({
            source: routeSource,
            style: (feature) => {
                const trafficLevel = feature.get("trafficLevel");

                const color =
                    trafficLevel === "High"
                        ? "#ef4444"
                        : trafficLevel === "Medium"
                            ? "#f59e0b"
                            : "#22c55e";

                return new Style({
                    stroke: new Stroke({
                        color,
                        width: 5,
                    }),
                });
            },
        });

        mapRef.current = new Map({
            target: mapElementRef.current,
            layers: [
                new TileLayer({
                    source: new OSM(),
                }),
                routeLayer,
                markerLayer,
            ],
            view: new View({
                center: fromLonLat([13.404954, 52.520008]),
                zoom: 13,
            }),
            controls: defaultControls({
                zoom: false,
            }),
        });

        return () => {
            mapRef.current?.setTarget(undefined);
            mapRef.current = null;
            markerSourceRef.current = null;
            routeSourceRef.current = null;
        };
    }, []);

    useEffect(() => {
        const markerSource = markerSourceRef.current;

        if (!markerSource) {
            return;
        }

        markerSource.clear();

        const activeDeliveries = deliveries.filter(
            (delivery) => delivery.currentPosition !== null,
        );

        for (const delivery of activeDeliveries) {
            if (!delivery.currentPosition) {
                continue;
            }

            const marker = new Feature({
                geometry: new Point(
                    fromLonLat([
                        delivery.currentPosition.longitude,
                        delivery.currentPosition.latitude,
                    ]),
                ),
                deliveryId: delivery.id,
                code: delivery.code,
            });

            markerSource.addFeature(marker);
        }
    }, [deliveries]);

    useEffect(() => {
        const routeSource = routeSourceRef.current;
        if (!routeSource) {
            return;
        }
        routeSource.clear();
        if (!selectedRoute || selectedRoute.points.length < 2) {
            return;
        }
        for (const trafficSegment of selectedRoute.trafficSegments) {
            const fromPoint = selectedRoute.points[trafficSegment.fromRoutePointIndex];
            const toPoint = selectedRoute.points[trafficSegment.toRoutePointIndex];
            if (!fromPoint || !toPoint) {
                continue;
            }
            const segmentFeature = new Feature({
                geometry: new LineString([
                    fromLonLat([fromPoint.longitude, fromPoint.latitude]),
                    fromLonLat([toPoint.longitude, toPoint.latitude]),
                ]),
                trafficLevel: trafficSegment.level,
            });
            routeSource.addFeature(segmentFeature);
        }
    }, [selectedRoute]);

    return <div className="delivery-map" ref={mapElementRef}/>;
}