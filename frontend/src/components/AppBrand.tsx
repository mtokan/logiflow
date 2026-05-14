import {Paper, Typography} from "@mui/material";
import "./AppBrand.css";

export function AppBrand() {
    return (
        <Paper className="app-brand" elevation={8}>
            <Typography variant="h5" component="h1">
                LogiFlow
            </Typography>
        </Paper>
    );
}