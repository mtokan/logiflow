export type GeoPoint = {
    latitude: number;
    longitude: number;
};

export type DeliveryState =
    | "Created"
    | "Planned"
    | "Assigned"
    | "InTransit"
    | "Arriving"
    | "Delivered"
    | "Closed";

export type TrafficLevel = "Low" | "Medium" | "High";

export type Delivery = {
    id: string;
    code: string;
    origin: GeoPoint;
    destination: GeoPoint;
    currentPosition: GeoPoint | null;
    state: DeliveryState;
    assignedVehicleId: string | null;
    routeId: string | null;
    etaSeconds: number | null;
    createdAt: string;
    updatedAt: string;
};

export type RoutePoint = {
    latitude: number;
    longitude: number;
    sequence: number;
    distanceFromPreviousMeters: number;
};

export type TrafficSegment = {
    fromRoutePointIndex: number;
    toRoutePointIndex: number;
    level: TrafficLevel;
    speedMultiplier: number;
};

export type DeliveryRoute = {
    id: string;
    deliveryId: string;
    points: RoutePoint[];
    trafficSegments: TrafficSegment[];
    totalDistanceMeters: number;
    estimatedDurationSeconds: number;
};