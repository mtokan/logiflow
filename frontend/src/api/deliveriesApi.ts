import {getJson, postJson} from "./httpClient";
import type {Delivery, DeliveryRoute} from "../types/delivery";

export type CreateDeliveryRequest = {
    code: string;
    originLatitude: number;
    originLongitude: number;
    destinationLatitude: number;
    destinationLongitude: number;
};

export type AssignVehicleRequest = {
    vehicleId: string;
};

export function getDeliveries(): Promise<Delivery[]> {
    return getJson<Delivery[]>("/api/deliveries");
}

export function getDelivery(id: string): Promise<Delivery> {
    return getJson<Delivery>(`/api/deliveries/${id}`);
}

export function getDeliveryRoute(id: string): Promise<DeliveryRoute> {
    return getJson<DeliveryRoute>(`/api/deliveries/${id}/route`);
}

export function createDelivery(
    request: CreateDeliveryRequest,
): Promise<Delivery> {
    return postJson<Delivery, CreateDeliveryRequest>("/api/deliveries", request);
}

export function planDelivery(id: string): Promise<Delivery> {
    return postJson<Delivery>(`/api/deliveries/${id}/plan`);
}

export function assignVehicle(
    id: string,
    request: AssignVehicleRequest,
): Promise<Delivery> {
    return postJson<Delivery, AssignVehicleRequest>(
        `/api/deliveries/${id}/assign-vehicle`,
        request,
    );
}

export function startDelivery(id: string): Promise<Delivery> {
    return postJson<Delivery>(`/api/deliveries/${id}/start`);
}

export function closeDelivery(id: string): Promise<Delivery> {
    return postJson<Delivery>(`/api/deliveries/${id}/close`);
}