<?php

class DataLoader
{
    public function GetPlayer($id)
    {
        return  array(
            'Id' => $id,
            'Name' => 'test' . $id,
            'Food' => 0,
            'Wood' => 0,
            'Metal' => 0
        );
    }
}
