import {AppBar, Toolbar, Typography} from "@mui/material";
import "./AppHeader.css";

export function AppHeader() {
    return (
        <AppBar position="absolute" elevation={0} className="app-header">
            <Toolbar>
                <Typography variant="h6" component="h1">LogiFlow</Typography>
            </Toolbar>
        </AppBar>
    );
}