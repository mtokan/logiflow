export type VehicleStatus =
    | "Available"
    | "Assigned"
    | "InTransit"
    | "Unavailable";

export type Vehicle = {
    id: string;
    name: string;
    status: VehicleStatus;
    assignedDeliveryId: string | null;
    isActive: boolean;
};