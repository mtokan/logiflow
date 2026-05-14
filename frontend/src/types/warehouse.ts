import type {GeoPoint} from "./delivery";

export type Warehouse = {
    id: string;
    name: string;
    location: GeoPoint;
};