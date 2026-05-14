import {useEffect, useState} from "react";
import {Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Typography,} from "@mui/material";
import "./CreateWarehouseDialog.css";

type CreateWarehouseDialogProps = {
    open: boolean;
    latitude: number | null;
    longitude: number | null;
    onClose: () => void;
    onSubmit: (name: string) => void;
};

export function CreateWarehouseDialog({open, latitude, longitude, onClose, onSubmit,}: CreateWarehouseDialogProps) {
    const [name, setName] = useState("");

    useEffect(() => {
        if (open) {
            setName("");
        }
    }, [open]);

    const handleSubmit = () => {
        const trimmedName = name.trim();

        if (!trimmedName) {
            return;
        }

        onSubmit(trimmedName);
    };

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
            <DialogTitle>Create Warehouse</DialogTitle>

            <DialogContent className="warehouse-dialog-content">
                <Typography variant="body2">
                    Latitude: {latitude?.toFixed(6) ?? "-"}
                </Typography>

                <Typography variant="body2">
                    Longitude: {longitude?.toFixed(6) ?? "-"}
                </Typography>

                <TextField
                    autoFocus
                    fullWidth
                    label="Warehouse name"
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                />
            </DialogContent>

            <DialogActions>
                <Button onClick={onClose}>Cancel</Button>
                <Button variant="contained" onClick={handleSubmit}>
                    Create
                </Button>
            </DialogActions>
        </Dialog>
    );
}