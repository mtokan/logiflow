import {useEffect} from "react";
import * as signalR from "@microsoft/signalr";
import type {
    DeliveryLogEventCreated,
    DeliveryPositionUpdated,
    DeliverySnapshotUpdated,
    VehicleSnapshotUpdated
} from "../../types/tracking.ts";


const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

type UseTrackingHubOptions = {
    onDeliveryPositionUpdated?: (update: DeliveryPositionUpdated) => void;
    onDeliveryLogEventCreated?: (event: DeliveryLogEventCreated) => void;
    onDeliverySnapshotUpdated?: (update: DeliverySnapshotUpdated) => void;
    onVehicleSnapshotUpdated?: (update: VehicleSnapshotUpdated) => void;
};

export function useTrackingHub({
                                   onDeliveryPositionUpdated,
                                   onDeliveryLogEventCreated,
                                   onDeliverySnapshotUpdated,
                                   onVehicleSnapshotUpdated
                               }: UseTrackingHubOptions) {
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`${API_BASE_URL}/hubs/tracking`)
            .withAutomaticReconnect()
            .build();

        connection.on("DeliveryPositionUpdated", (update: DeliveryPositionUpdated) => {
            onDeliveryPositionUpdated?.(update);
        });

        connection.on("DeliveryLogEventCreated", (event: DeliveryLogEventCreated) => {
            onDeliveryLogEventCreated?.(event);
        });

        connection.on("DeliverySnapshotUpdated", (update: DeliverySnapshotUpdated) => {
            onDeliverySnapshotUpdated?.(update);
        });

        connection.on("VehicleSnapshotUpdated", (update: VehicleSnapshotUpdated) => {
            onVehicleSnapshotUpdated?.(update);
        });

        connection.start().catch((error: unknown) => {
            console.error("SignalR connection failed", error);
        });

        return () => {
            connection.stop().catch((error: unknown) => {
                console.error("SignalR disconnection failed", error);
            });
        };
    }, [
        onDeliveryPositionUpdated,
        onDeliveryLogEventCreated,
        onDeliverySnapshotUpdated,
        onVehicleSnapshotUpdated
    ]);
}