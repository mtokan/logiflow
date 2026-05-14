import {getJson, postJson} from "./httpClient";
import type {Warehouse} from "../types/warehouse";

export type CreateWarehouseRequest = {
    name: string;
    latitude: number;
    longitude: number;
};

export function getWarehouses(): Promise<Warehouse[]> {
    return getJson<Warehouse[]>("/api/warehouses");
}

export function createWarehouse(request: CreateWarehouseRequest): Promise<Warehouse> {
    return postJson<Warehouse, CreateWarehouseRequest>("/api/warehouses", request);
}