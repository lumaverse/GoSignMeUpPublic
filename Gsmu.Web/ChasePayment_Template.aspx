<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChasePayment_Template.aspx.cs" Inherits="Gsmu.Web.ChasePayment_Template" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    	
		<script src="https://dev250.gosignmeup.com/bundles/jquery?v=Nsx6sB8o7LJCR51P5u-dfe-6LePnt87pD0P5VTJ-0wI1"></script>
		<script src="https://dev250.gosignmeup.com/Scripts/jquery.placeholder.min.js"></script>
		<script src="https://dev250.gosignmeup.com/bundles/jqueryval?v=kO9SZjRLUEvNZbFlwaT1hsJ0t0ngQHk32HeNdumCbRM1"></script>
		<script src="https://dev250.gosignmeup.com/bundles/jqueryui?v=G-dvGKiPxJe_4ZQfKCHav6gX76lKgX0BTz2Hj2T6dfc1"></script>
    
	<script>
	    $("#submitButton").click(function () {
	        $("input[type=submit]").attr("disabled", "disabled");
	    });
	</script>
    <style>
        label {
            font-family: Sans-serif;
            font-size: 12px;
            text-align: right;
        }
		
		.main{
			margin-bottom:10px;
		}
		
		table{
			margin-left:250px; 
			width:100px;
		}
		
		input{
				border-color: #B5B8C8;
				border-style: solid;
				border-width: 1px;
				color: #000000;
				padding: 1px 3px 2px;
		}
		
		body{
			margin-left:150px;
			background: url("../images/layout/bg.jpg") repeat-x scroll left top #FFFFFF;
		}
		.submitButton{
			background: -moz-linear-gradient(center top , #97C865 0%, #44933D 100%) repeat scroll 0 0 rgba(0, 0, 0, 0);
			border-color: #3D7530;
			box-shadow: 0 1px 0 #D8E994 inset;
			color: #FFFFFF;
		}
        		
		.disabled{
		    background-color:gray;
            background: -moz-linear-gradient(center top , #8b8989 0%, #8b8989 100%) repeat scroll 0 0 rgba(0, 0, 0, 0);
            color: GrayText;
		}

        button[disabled=disabled], button:disabled,.submitButton:disabled {
            background-color:gray;
            background: -moz-linear-gradient(center top , #8b8989 0%, #8b8989 100%) repeat scroll 0 0 rgba(0, 0, 0, 0);
            color: GrayText;
        }
    </style>
</head>
<body>
    <div id="divHeader" style="" runat="server"></div>
<div>
               [[FORM INSERT]]
</div>
</body>
</html>
