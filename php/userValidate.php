<?php

ini_set('display_errors', true);
error_reporting(E_ALL);

$user=$_GET['user'];
$time = time();

header('Content-Type:text/html');

$lines = file("mining_users.txt", FILE_IGNORE_NEW_LINES);
$pos=0;
$found=0;

while($found == 0 && $pos < count($lines)){
    if($user == $lines[$pos]){
        $found = 1;
    }
    $pos++;
}

if($found == 1){
    echo "yay";
}else{
    echo "boo";
}


?>

