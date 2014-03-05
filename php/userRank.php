<?php

ini_set('display_errors', true);
error_reporting(E_ALL);

$user=$_GET['user'];

header('Content-Type:text/html');
$fileName= "btc_balances.txt";

$lines = file($fileName, FILE_IGNORE_NEW_LINES);
$pos=0;
$found=0;
$bal = array();

// build balances
for($x=0; $x < count($lines); $x++){
    $tmp = explode(",",$lines[$x]);
    $bal[$tmp[0]] = $tmp[1];
}

// sort and return index and count ;)
arsort($bal,1);
$len = count($bal);
if (array_key_exists($user,$bal))
{
   $pos = array_search($user, array_keys($bal));
   $pos = $pos + 1;
   echo $pos." of ".$len;
}
else
{
  echo "0 of 0";
}

?>

