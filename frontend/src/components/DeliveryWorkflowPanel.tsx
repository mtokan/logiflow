import CloseIcon from "@mui/icons-material/Close";
import {Box, Button, IconButton, Paper, Step, StepLabel, Stepper, Typography,} from "@mui/material";
import type {Delivery, DeliveryState} from "../types/delivery";
import "./DeliveryWorkflowPanel.css";

type DeliveryWorkflowPanelProps = {
    delivery: Delivery | null;
    onClose: () => void;
};

const deliverySteps: DeliveryState[] = [
    "Created",
    "Planned",
    "Assigned",
    "InTransit",
    "Arriving",
    "Delivered",
    "Closed",
];

function getActiveStep(state: DeliveryState): number {
    return deliverySteps.indexOf(state);
}

function getNextActionLabel(state: DeliveryState): string | null {
    return state === "Created"
        ? "Plan Delivery"
        : state === "Planned"
            ? "Assign Vehicle"
            : state === "Assigned"
                ? "Start Delivery"
                : state === "Delivered"
                    ? "Close Delivery"
                    : null;
}

export function DeliveryWorkflowPanel({delivery, onClose,}: DeliveryWorkflowPanelProps) {
    if (!delivery) {
        return null;
    }

    const activeStep = getActiveStep(delivery.state);
    const nextActionLabel = getNextActionLabel(delivery.state);

    return (
        <Paper className="delivery-workflow-panel" elevation={8}>
            <Box className="workflow-panel-header">
                <Box>
                    <Typography variant="h6">{delivery.code}</Typography>
                    <Typography variant="body2" className="workflow-panel-state">
                        Current state: {delivery.state}
                    </Typography>
                </Box>

                <IconButton onClick={onClose} aria-label="Close delivery panel">
                    <CloseIcon/>
                </IconButton>
            </Box>

            <Stepper activeStep={activeStep} alternativeLabel className="delivery-stepper">
                {deliverySteps.map((step) => (
                    <Step key={step}>
                        <StepLabel>{step}</StepLabel>
                    </Step>
                ))}
            </Stepper>

            <Box className="workflow-panel-footer">
                <Box className="workflow-panel-summary">
                    <Typography variant="body2">
                        ETA: {delivery.etaSeconds?.toFixed(0) ?? "-"}s
                    </Typography>

                    <Typography variant="body2">
                        Vehicle: {delivery.assignedVehicleId ? "Assigned" : "Unassigned"}
                    </Typography>
                </Box>

                {nextActionLabel && (
                    <Button variant="contained" className="workflow-action-button">
                        {nextActionLabel}
                    </Button>
                )}
            </Box>
        </Paper>
    );
}