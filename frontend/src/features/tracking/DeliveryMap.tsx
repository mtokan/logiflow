import {useEffect, useRef} from "react";
import Map from "ol/Map";
import View from "ol/View";
import Feature from "ol/Feature";
import Point from "ol/geom/Point";
import TileLayer from "ol/layer/Tile";
import VectorLayer from "ol/layer/Vector";
import OSM from "ol/source/OSM";
import VectorSource from "ol/source/Vector";
import {fromLonLat, toLonLat} from "ol/proj";
import {Circle, Fill, Stroke, Style} from "ol/style";
import type {Delivery, DeliveryRoute} from "../../types/delivery";
import {LineString} from "ol/geom";
import {defaults as defaultControls} from "ol/control/defaults";
import type {Warehouse} from "../../types/warehouse.ts"
import Icon from "ol/style/Icon"
import {Overlay} from "ol"

type DeliveryMapProps = {
    deliveries: Delivery[];
    warehouses: Warehouse[];
    selectedRoute: DeliveryRoute | null;
    isSelectingWarehouse?: boolean;
    onMapClick?: (latitude: number, longitude: number) => void;
};

export function DeliveryMap({
    deliveries,
    warehouses,
    selectedRoute,
    isSelectingWarehouse,
    onMapClick
}: DeliveryMapProps) {
    const mapElementRef = useRef<HTMLDivElement | null>(null);
    const mapRef = useRef<Map | null>(null);
    const routeSourceRef = useRef<VectorSource | null>(null);
    const markerSourceRef = useRef<VectorSource | null>(null);
    const isSelectingWarehouseRef = useRef(isSelectingWarehouse);
    const onMapClickRef = useRef(onMapClick);
    const warehouseSourceRef = useRef<VectorSource | null>(null);
    const tooltipElementRef = useRef<HTMLDivElement | null>(null);
    const tooltipOverlayRef = useRef<Overlay | null>(null);

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

        const warehouseSource = new VectorSource();
        warehouseSourceRef.current = warehouseSource;

        const warehouseLayer = new VectorLayer({
            source: warehouseSource,
            style: new Style({
                image: new Icon({
                    src: "/warehouse-marker.svg",
                    anchor: [0.5, 1],
                    scale: 0.05,
                }),
            }),
        });

        mapRef.current = new Map({
            target: mapElementRef.current,
            layers: [
                new TileLayer({
                    source: new OSM(),
                }),
                routeLayer,
                markerLayer,
                warehouseLayer,
            ],
            view: new View({
                center: fromLonLat([13.404954, 52.520008]),
                zoom: 13,
            }),
            controls: defaultControls({
                zoom: false,
            }),
        });

        const tooltipOverlay = new Overlay({
            element: tooltipElementRef.current ?? undefined,
            offset: [12, 0],
            positioning: "center-left",
        });

        tooltipOverlayRef.current = tooltipOverlay;
        mapRef.current.addOverlay(tooltipOverlay);

        mapRef.current.on("singleclick", (event) => {
            if (!isSelectingWarehouseRef.current || !onMapClickRef.current) {
                return;
            }
            const [longitude, latitude] = toLonLat(event.coordinate);
            onMapClickRef.current(latitude, longitude);
        });

        mapRef.current.on("pointermove", (event) => {
            const map = mapRef.current;
            const tooltipElement = tooltipElementRef.current;
            const tooltipOverlay = tooltipOverlayRef.current;

            if (!map || !tooltipElement || !tooltipOverlay) {
                return;
            }

            const feature = map.forEachFeatureAtPixel(event.pixel, (feature) => feature);

            const warehouseName = feature?.get("warehouseName");

            if (!warehouseName) {
                tooltipOverlay.setPosition(undefined);
                tooltipElement.style.display = "none";
                return;
            }

            tooltipElement.textContent = `Warehouse: ${warehouseName}`;
            tooltipElement.style.display = "block";
            tooltipOverlay.setPosition(event.coordinate);
        });

        return () => {
            mapRef.current?.setTarget(undefined);
            mapRef.current = null;
            markerSourceRef.current = null;
            routeSourceRef.current = null;
            warehouseSourceRef.current = null;
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

    useEffect(() => {
        isSelectingWarehouseRef.current = isSelectingWarehouse;
    }, [isSelectingWarehouse]);

    useEffect(() => {
        onMapClickRef.current = onMapClick;
    }, [onMapClick]);

    useEffect(() => {
        const mapElement = mapElementRef.current;
        if (!mapElement) {
            return;
        }
        mapElement.style.cursor = isSelectingWarehouse ? "crosshair" : "default";
    }, [isSelectingWarehouse]);

    useEffect(() => {
        const warehouseSource = warehouseSourceRef.current;
        if (!warehouseSource) {
            return;
        }
        warehouseSource.clear();

        for (const warehouse of warehouses) {
            const marker = new Feature({
                geometry: new Point(
                    fromLonLat([
                        warehouse.location.longitude,
                        warehouse.location.latitude,
                    ]),
                ),
                warehouseId: warehouse.id,
                warehouseName: warehouse.name,
            });

            warehouseSource.addFeature(marker);
        }
    }, [warehouses]);

    return (
        <>
            <div className="delivery-map" ref={mapElementRef}/>
            <div ref={tooltipElementRef} className="map-tooltip"/>
        </>
    );
}