<?php

$xmlData = file_get_contents('D:\xmlassignment\Assets\playerdata.xml');

$playerData = simplexml_load_string($xmlData);

$playerName = $playerData->playerName;
$score = $playerData->score;

$filename = "playerdata.xml";
$file = fopen($filename, "a");
fwrite($file, "$playerName,$score\n");
fclose($file);

echo "Received player data: $playerName,$score";

?>