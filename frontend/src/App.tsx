import {useEffect, useState} from "react";
import {getDeliveries} from "./api/deliveriesApi";
import {getVehicles} from "./api/vehiclesApi";
import {AppBrand} from "./components/AppBrand";
import {DeliveryListPanel} from "./components/DeliveryListPanel";
import {DeliveryWorkflowPanel} from "./components/DeliveryWorkflowPanel";
import {DeliveryMap} from "./features/tracking/DeliveryMap";
import type {Delivery} from "./types/delivery";
import type {Vehicle} from "./types/vehicle";

function App() {
    const [deliveries, setDeliveries] = useState<Delivery[]>([]);
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [selectedDeliveryId, setSelectedDeliveryId] = useState<string | null>(null);

    useEffect(() => {
        getDeliveries().then(setDeliveries);
        getVehicles().then(setVehicles);
    }, []);

    const selectedDelivery =
        deliveries.find((delivery) => delivery.id === selectedDeliveryId) ?? null;

    return (
        <>
            <DeliveryMap deliveries={deliveries} selectedRoute={null}/>

            <AppBrand/>

            <DeliveryListPanel
                deliveries={deliveries}
                selectedDeliveryId={selectedDeliveryId}
                onSelectDelivery={setSelectedDeliveryId}
            />

            <DeliveryWorkflowPanel
                delivery={selectedDelivery}
                onClose={() => setSelectedDeliveryId(null)}
            />
        </>
    );
}

export default App;