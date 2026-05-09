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
import type {Delivery} from "../../types/delivery";

type DeliveryMapProps = {
    deliveries: Delivery[];
};

export function DeliveryMap({deliveries}: DeliveryMapProps) {
    const mapElementRef = useRef<HTMLDivElement | null>(null);
    const mapRef = useRef<Map | null>(null);
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

        mapRef.current = new Map({
            target: mapElementRef.current,
            layers: [
                new TileLayer({
                    source: new OSM(),
                }),
                markerLayer,
            ],
            view: new View({
                center: fromLonLat([13.404954, 52.520008]),
                zoom: 13,
            }),
        });

        return () => {
            mapRef.current?.setTarget(undefined);
            mapRef.current = null;
            markerSourceRef.current = null;
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

    return <div className="delivery-map" ref={mapElementRef}/>;
}