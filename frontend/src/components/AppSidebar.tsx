import {useState} from "react";
import {Box, Collapse, Drawer, List, ListItemButton, ListItemText,} from "@mui/material";
import {ExpandLess, ExpandMore} from "@mui/icons-material";
import type {Delivery} from "../types/delivery";
import type {Vehicle} from "../types/vehicle";
import "./AppSidebar.css";

type AppSidebarProps = {
    deliveries: Delivery[];
    vehicles: Vehicle[];
    selectedDeliveryId: string | null;
    onSelectDelivery: (deliveryId: string) => void;
};

export function AppSidebar({deliveries, vehicles, selectedDeliveryId, onSelectDelivery,}: AppSidebarProps) {
    const [deliveriesOpen, setDeliveriesOpen] = useState(true);
    const [vehiclesOpen, setVehiclesOpen] = useState(false);

    return (
        <Drawer variant="permanent" anchor="left" className="app-sidebar">
            <Box className="sidebar-content">
                <List disablePadding>
                    <ListItemButton onClick={() => setDeliveriesOpen((open) => !open)}>
                        <ListItemText primary="Deliveries"/>
                        {deliveriesOpen ? <ExpandLess/> : <ExpandMore/>}
                    </ListItemButton>

                    <Collapse in={deliveriesOpen} timeout="auto" unmountOnExit>
                        <List component="div" disablePadding>
                            {deliveries.map((delivery) => (
                                <ListItemButton
                                    key={delivery.id}
                                    className="nested-list-item"
                                    selected={selectedDeliveryId === delivery.id}
                                    onClick={() => onSelectDelivery(delivery.id)}
                                >
                                    <ListItemText primary={delivery.code} secondary={delivery.state}/>
                                </ListItemButton>
                            ))}
                        </List>
                    </Collapse>

                    <ListItemButton onClick={() => setVehiclesOpen((open) => !open)}>
                        <ListItemText primary="Vehicles"/>
                        {vehiclesOpen ? <ExpandLess/> : <ExpandMore/>}
                    </ListItemButton>

                    <Collapse in={vehiclesOpen} timeout="auto" unmountOnExit>
                        <List component="div" disablePadding>
                            {vehicles.map((vehicle) => (
                                <ListItemButton key={vehicle.id} className="nested-list-item">
                                    <ListItemText
                                        primary={vehicle.name}
                                        secondary={vehicle.status}
                                    />
                                </ListItemButton>
                            ))}
                        </List>
                    </Collapse>
                </List>
            </Box>
        </Drawer>
    );
}