import {useCallback, useEffect, useState} from "react";
import {getDeliveries} from "../api/deliveriesApi";
import {getVehicles} from "../api/vehiclesApi";
import type {Delivery} from "../types/delivery";
import type {Vehicle} from "../types/vehicle";
import type {
    DeliveryLogEventCreated,
    DeliveryPositionUpdated,
    DeliverySnapshotUpdated,
    VehicleSnapshotUpdated
} from "../types/tracking.ts";
import {useTrackingHub} from "../features/tracking/useTrackingHub.ts";
import {DeliveryMap} from "../features/tracking/DeliveryMap.tsx";

export function DashboardPage() {
    const [deliveries, setDeliveries] = useState<Delivery[]>([]);
    const [vehicles, setVehicles] = useState<Vehicle[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [lastEvent, setLastEvent] = useState<DeliveryLogEventCreated | null>(null);

    useEffect(() => {
        Promise.all([getDeliveries(), getVehicles()])
            .then(([deliveries, vehicles]) => {
                setDeliveries(deliveries);
                setVehicles(vehicles);
            })
            .catch((error: unknown) => {
                setError(error instanceof Error ? error.message : "Unknown error");
            });
    }, []);

    const upsertDelivery = useCallback((delivery: Delivery) => {
        setDeliveries((current) => {
            const exists = current.some((item) => item.id === delivery.id);

            if (!exists) {
                return [delivery, ...current];
            }

            return current.map((item) => (item.id === delivery.id ? delivery : item));
        });
    }, []);

    const upsertVehicle = useCallback((vehicle: Vehicle) => {
        setVehicles((current) => {
            const exists = current.some((item) => item.id === vehicle.id);

            if (!exists) {
                return [vehicle, ...current];
            }

            return current.map((item) => (item.id === vehicle.id ? vehicle : item));
        });
    }, []);

    const handleDeliverySnapshotUpdated = useCallback(
        (update: DeliverySnapshotUpdated) => {
            upsertDelivery(update.delivery);
        },
        [upsertDelivery],
    );

    const handleVehicleSnapshotUpdated = useCallback(
        (update: VehicleSnapshotUpdated) => {
            upsertVehicle(update.vehicle);
        },
        [upsertVehicle],
    );

    const handleDeliveryPositionUpdated = useCallback(
        (update: DeliveryPositionUpdated) => {
            setDeliveries((current) =>
                current.map((delivery) =>
                    delivery.id === update.deliveryId
                        ? {
                            ...delivery,
                            currentPosition: update.position,
                            etaSeconds: update.etaSeconds,
                            state: update.state,
                            assignedVehicleId: update.vehicleId,
                        }
                        : delivery,
                ),
            );
        },
        [],
    );

    const handleDeliveryLogEventCreated = useCallback(
        (event: DeliveryLogEventCreated) => {
            setLastEvent(event);
        },
        [],
    );

    useTrackingHub({
        onDeliveryPositionUpdated: handleDeliveryPositionUpdated,
        onDeliveryLogEventCreated: handleDeliveryLogEventCreated,
        onDeliverySnapshotUpdated: handleDeliverySnapshotUpdated,
        onVehicleSnapshotUpdated: handleVehicleSnapshotUpdated,
    });

    return (
        <main className="dashboard">
            <header className="dashboard-header">
                <div>
                    <h1>LogiFlow</h1>
                    <p>Real-time logistics tracking dashboard</p>
                </div>
            </header>

            {error && <div className="error-box">{error}</div>}

            <section className="summary-grid">
                <article className="summary-card">
                    <span>Deliveries</span>
                    <strong>{deliveries.length}</strong>
                </article>

                <article className="summary-card">
                    <span>Vehicles</span>
                    <strong>{vehicles.length}</strong>
                </article>

                <article className="summary-card">
                    <span>Active</span>
                    <strong>
                        {
                            deliveries.filter((delivery) =>
                                ["Assigned", "InTransit", "Arriving"].includes(delivery.state),
                            ).length
                        }
                    </strong>
                </article>
            </section>

            {lastEvent && (
                <section className="panel latest-event">
                    <h2>Latest Event</h2>
                    <p>{lastEvent.message}</p>
                </section>
            )}

            <section className="panel map-panel">
                <h2>Live Map</h2>
                <DeliveryMap deliveries={deliveries}/>
            </section>

            <section className="content-grid">
                <section className="panel">
                    <h2>Deliveries</h2>

                    {deliveries.length === 0 ? (
                        <p className="muted">No deliveries yet.</p>
                    ) : (
                        <div className="list">
                            {deliveries.map((delivery) => (
                                <article key={delivery.id} className="list-item">
                                    <div>
                                        <strong>{delivery.code}</strong>
                                        <p>{delivery.state}</p>
                                    </div>
                                    <span>{delivery.etaSeconds?.toFixed(0) ?? "-"}s</span>
                                </article>
                            ))}
                        </div>
                    )}
                </section>

                <section className="panel">
                    <h2>Vehicles</h2>

                    {vehicles.length === 0 ? (
                        <p className="muted">No vehicles yet.</p>
                    ) : (
                        <div className="list">
                            {vehicles.map((vehicle) => (
                                <article key={vehicle.id} className="list-item">
                                    <div>
                                        <strong>{vehicle.name}</strong>
                                        <p>{vehicle.status}</p>
                                    </div>
                                </article>
                            ))}
                        </div>
                    )}
                </section>
            </section>
        </main>
    );
}