import AddLocationAltIcon from "@mui/icons-material/AddLocationAlt";
import {IconButton, Tooltip} from "@mui/material";
import "./AddWarehouseButton.css";

type AddWarehouseButtonProps = { onClick: () => void; };

export function AddWarehouseButton({onClick}: AddWarehouseButtonProps) {
    return (
        <Tooltip title="Add warehouse" placement="right">
            <IconButton className="add-warehouse-button" onClick={onClick} aria-label="Add warehouse">
                <AddLocationAltIcon/>
            </IconButton>
        </Tooltip>
    );
}