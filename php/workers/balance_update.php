<?php

//ini_set('display_errors', true);
//error_reporting(E_ALL);

//$wallet=$_GET['wallet'];

//header('Content-Type:text/html');

//$result = FetchProfitSwitchBalance($wallet);

//echo json_encode($result);

/*
	25/02/2014
	
	-- Current Data Structure --
	
	<b>Bitcoins sent to you:</b> 0.00120081<br>
    <b>Bitcoins earned (not yet sent):</b> 0.00099282<br>
    <b>Bitcoins unconverted (approximate):</b> 0.00000217<br>

*/

function FetchProfitSwitchBalance($wallet){
	$urlBase = "http://wafflepool.com/miner/";

	$contents = file_get_contents($urlBase.$wallet);

	$sentBtc = ExtractSentBtc($contents);
	$earnedBtc = ExtractEarnedBtc($contents);
	
	return  $sentBtc + $earnedBtc;
	
	//return new ProfitSwitchBalance($sentBtc, $earnedBtc);
}

// sent bit coins ;)
function ExtractEarnedBtc($contents){

	$earnedBtcValue = "<b>Bitcoins earned (not yet sent):</b>";
	
	return ExtractBtcValue($contents, $earnedBtcValue);
}

// sent bit coins ;)
function ExtractSentBtc($contents){

	$sendBtcValue = "<b>Bitcoins sent to you:</b>";
		
	return ExtractBtcValue($contents, $sendBtcValue);
}

function ExtractBtcValue($contents, $magicString){
	$idx = stripos($contents, $magicString);
	
	if($idx >= 0){
		$endIdx = stripos($contents, "<br>", $idx);
		
		if($endIdx >= 0){
				$idx += strlen($magicString);
				$len = $endIdx - $idx;
				$sendBtc = substr($contents, $idx, $len);
				$sendBtc = trim($sendBtc);
				
				return $sendBtc;
		}
	}
	
	return "0.0";
}

class ProfitSwitchBalance{

	public $SentBtc;
	
	public $EarnedBtc;
		
	function __construct($sent, $earned){
		
		$this->SentBtc = $sent;
		
		$this->EarnedBtc = $earned;
	}
	
	function TotalBtc(){
		return $this->SentBtc + $this->EarnedBtc;
	}
}

?>

