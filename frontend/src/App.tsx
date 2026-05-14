import {useEffect, useState} from "react";
import {getDeliveries} from "./api/deliveriesApi";
import {getVehicles} from "./api/vehiclesApi";
import {AppBrand} from "./components/AppBrand";
import {DeliveryListPanel} from "./components/DeliveryListPanel";
import {DeliveryWorkflowPanel} from "./components/DeliveryWorkflowPanel";
import {DeliveryMap} from "./features/tracking/DeliveryMap";
import type {Delivery} from "./types/delivery";
import type {Vehicle} from "./types/vehicle";
import {createWarehouse, getWarehouses} from "./api/warehousesApi.ts"
import type {Warehouse} from "./types/warehouse.ts"
import {AddWarehouseButton} from "./components/AddWarehouseButton.tsx"
import {CreateWarehouseDialog} from "./components/CreateWarehouseDialog.tsx"

function App() {
    const [deliveries, setDeliveries] = useState<Delivery[]>([]);
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [selectedDeliveryId, setSelectedDeliveryId] = useState<string | null>(null);
    const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
    const [isAddingWarehouse, setIsAddingWarehouse] = useState(false);
    const [warehouseDraftPosition, setWarehouseDraftPosition] =
        useState<{ latitude: number; longitude: number; } | null>(null);

    useEffect(() => {
        getDeliveries().then(setDeliveries);
        getVehicles().then(setVehicles);
        getWarehouses().then(setWarehouses);
    }, []);

    const selectedDelivery =
        deliveries.find((delivery) => delivery.id === selectedDeliveryId) ?? null;

    return (
        <>
            <DeliveryMap
                deliveries={deliveries}
                warehouses={warehouses}
                selectedRoute={null}
                isSelectingWarehouse={isAddingWarehouse}
                onMapClick={(latitude, longitude) => {
                    setWarehouseDraftPosition({latitude, longitude});
                    setIsAddingWarehouse(false);
                }}
            />

            <AppBrand/>

            <AddWarehouseButton onClick={() => {
                setIsAddingWarehouse(true);
                setWarehouseDraftPosition(null);
            }}
            />

            <DeliveryListPanel
                deliveries={deliveries}
                selectedDeliveryId={selectedDeliveryId}
                onSelectDelivery={setSelectedDeliveryId}
            />

            <DeliveryWorkflowPanel
                delivery={selectedDelivery}
                onClose={() => setSelectedDeliveryId(null)}
            />

            <CreateWarehouseDialog
                open={warehouseDraftPosition !== null}
                latitude={warehouseDraftPosition?.latitude ?? null}
                longitude={warehouseDraftPosition?.longitude ?? null}
                onClose={() => setWarehouseDraftPosition(null)}
                onSubmit={async (name) => {
                    if (!warehouseDraftPosition) {
                        return;
                    }
                    const warehouse = await createWarehouse({
                        name,
                        latitude: warehouseDraftPosition.latitude,
                        longitude: warehouseDraftPosition.longitude,
                    });
                    setWarehouses((current) => [warehouse, ...current]);
                    setWarehouseDraftPosition(null);
                }}
            />
        </>
    );
}

export default App;