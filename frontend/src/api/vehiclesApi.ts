import {getJson, postJson} from "./httpClient";
import type {Vehicle} from "../types/vehicle";

export type CreateVehicleRequest = {
    name: string;
};

export function getVehicles(): Promise<Vehicle[]> {
    return getJson<Vehicle[]>("/api/vehicles");
}

export function createVehicle(request: CreateVehicleRequest): Promise<Vehicle> {
    return postJson<Vehicle, CreateVehicleRequest>("/api/vehicles", request);
}