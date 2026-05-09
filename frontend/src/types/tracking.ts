import type {Delivery, DeliveryState, GeoPoint} from "./delivery";
import type {Vehicle} from "./vehicle";

export type DeliveryPositionUpdated = {
    deliveryId: string;
    vehicleId: string;
    position: GeoPoint;
    speedMetersPerSecond: number;
    etaSeconds: number;
    progressPercent: number;
    state: DeliveryState;
    timestamp: string;
};

export type DeliveryLogEventCreated = {
    eventId: string;
    deliveryId: string;
    type: string;
    fromState: DeliveryState | null;
    toState: DeliveryState | null;
    message: string;
    createdAt: string;
};

export type DeliverySnapshotUpdated = {
    delivery: Delivery;
    reason: string;
    timestamp: string;
};

export type VehicleSnapshotUpdated = {
    vehicle: Vehicle;
    reason: string;
    timestamp: string;
};