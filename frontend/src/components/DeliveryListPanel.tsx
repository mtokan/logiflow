import {ExpandLess, ExpandMore} from "@mui/icons-material";
import {Box, Collapse, List, ListItemButton, ListItemText, Paper, Typography,} from "@mui/material";
import {useState} from "react";
import type {Delivery} from "../types/delivery";
import "./DeliveryListPanel.css";

type DeliveryListPanelProps = {
    deliveries: Delivery[];
    selectedDeliveryId: string | null;
    onSelectDelivery: (deliveryId: string) => void;
};

export function DeliveryListPanel({
                                      deliveries,
                                      selectedDeliveryId,
                                      onSelectDelivery,
                                  }: DeliveryListPanelProps) {
    const [open, setOpen] = useState(false);

    return (
        <Paper className="delivery-list-panel" elevation={8}>
            <List disablePadding>
                <ListItemButton
                    className="delivery-list-toggle"
                    onClick={() => setOpen((current) => !current)}
                >
                    <Box className="delivery-list-title">
                        <Typography variant="h6">Deliveries</Typography>
                        <Typography variant="body2">{deliveries.length}</Typography>
                    </Box>

                    {open ? <ExpandLess/> : <ExpandMore/>}
                </ListItemButton>

                <Collapse in={open} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding className="delivery-items">
                        {deliveries.map((delivery) => (
                            <ListItemButton
                                key={delivery.id}
                                selected={selectedDeliveryId === delivery.id}
                                onClick={() => onSelectDelivery(delivery.id)}
                                className="delivery-list-item"
                            >
                                <ListItemText
                                    primary={delivery.code}
                                    secondary={delivery.state}
                                />
                            </ListItemButton>
                        ))}
                    </List>
                </Collapse>
            </List>
        </Paper>
    );
}