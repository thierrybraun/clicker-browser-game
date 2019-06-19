<?php
class Debug
{
    private $cityManager;

    public function __construct(CityManager $cityManager)
    {
        $this->cityManager = $cityManager;
    }

    public function createTerrainVisualization(): string
    {
        $terrain = $this->cityManager->createTerrain(mt_rand());
        $res = '<body><table style="width: 100%;height: 100%;" cellspacing="0">';
        for ($y = 0; $y < count($terrain); $y += 1) {
            $res .= '<tr>';
            for ($x = 0; $x < count($terrain[$y]); $x += 1) {
                $col = '0,0,0';
                switch ($terrain[$y][$x][0]) {
                    case FieldType::Water:
                        $col = '0,0,255';
                        break;
                    case FieldType::Plain:
                        $col = '0,255,0';
                        break;
                    case FieldType::Hills:
                        $col = '128,128,128';
                        break;
                }

                $cont = '&nbsp;';
                if (count($terrain[$y][$x]) > 1) {
                    switch ($terrain[$y][$x][1]) {
                        case ResourceType::Apples:
                            $cont = 'A';
                            break;
                        case ResourceType::Fish:
                            $cont = 'F';
                            break;
                        case ResourceType::Forest:
                            $col = '0,128,0';
                            break;
                        case ResourceType::Ore:
                            $cont = 'O';
                            break;
                    }
                }

                $res .= "<td style=\"background-color: rgb($col)\">$cont</td>";
            }
            $res .= '</tr>';
        }
        $res .= '</table></body>';
        return $res;
    }
}
