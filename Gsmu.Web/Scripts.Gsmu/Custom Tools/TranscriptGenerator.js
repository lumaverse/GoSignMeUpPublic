$(function () {
    $("#divHeader").resizable({
        containment: "#PDFDocs"
    });



    $("#divHeader").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {
         //   alert(ui.position.left.toFixed(2) + "," + ui.position.top.toFixed(2) + "," + $(this).width().toFixed(2) + "," + $(this).height().toFixed(2));
        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });

    //Optional Text

    $("#divOptionalText").resizable({
        containment: "#PDFDocs"
    });



    $("#divOptionalText").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });


    $("#coursedetails").resizable({
        containment: "#PDFDocs"
    });



    $("#coursedetails").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {
         
        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });


    //Student Name

    $("#divStudentName").resizable({
        containment: "#PDFDocs"
    });



    $("#divStudentName").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });


    //Studen Address

    $("#divStudentAddress").resizable({
        containment: "#PDFDocs"
    });



    $("#divStudentAddress").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });


    //Custom Field 1

    $("#divCustomField1").resizable({
        containment: "#PDFDocs"
    });



    $("#divCustomField1").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });

    //Custom Field 2

    $("#divCustomField2").resizable({
        containment: "#PDFDocs"
    });



    $("#divCustomField2").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {
         
        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });

    //Custom Field 3

    $("#divCustomField3").resizable({
        containment: "#PDFDocs"
    });



    $("#divCustomField3").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });

    //Date Printed
    $("#coursedetails").resizable({
        containment: "#PDFDocs"
    });



    $("#coursedetails").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    });



    $("#divDatePrint").resizable({
        containment: "#PDFDocs"
    });
    $("#divDatePrint").draggable({
        containment: "#PDFDocs",
        cursor: 'move',

        drag: function (event, ui) {

        }
    }).parent().resizable({
        containment: "#PDFDocs"
    }); 

    //Dialog boxes
    $("#btnPdfSet").click(function () {
        $("#dialog-form-pdf").dialog("open");
    });
    $("#dialog-form-pdf").dialog({
        autoOpen: false,

        width: 580,
        modal: true,
        buttons: {
            "Preview": function () {

                $("#PDFDocs").attr("style", " background-color:white; width:" + $("#txtboundx").val() + ";height:" + $("#txtboundy").val() + ";margin:18px;");
                $("#txtwidth").val(parseInt($("#txtboundx").val()) + parseInt($("#txtleft").val()) + parseInt($("#txtright").val()));
                $("#txtheight").val(parseInt($("#txtboundy").val()) + parseInt($("#txttop").val()) + parseInt($("#txtbottom").val()));
                $("#PDFFrame").css('width', $("#txtwidth").val());

                $("#PDFDocs").css('width', $("#txtboundx").val());
                $("#PDFDocs").css('height', $("#txtboundy").val());
                $("#PDFDocs").css('margin-top', $("#txttop").val());
                $("#PDFDocs").css('margin-bottom', $("#txtbottom").val());
                $("#PDFDocs").css('margin-left', $("#txtleft").val());
                $("#PDFDocs").css('margin-right', $("#txtright").val());
                $('#certtitle').val($("#txtCertTitle").val());
                $('#certheight').val($("#txtheight").val());
                $('#certwidth').val($("#txtwidth").val());
                $('#marginTop').val($("#txttop").val());
                $('#marginBottom').val($("#txtbottom").val());
                $('#marginLeft').val($("#txtleft").val());
                $('#marginRight').val($("#txtright").val());
                $('#boundX').val($("#txtboundx").val());
                $('#boundY').val($("#txtboundy").val());
                $("#PDFFrame").css('height', $("#txtheight").val());
               
                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });



    $("#btnTransSet").click(function () {
        SetCheckboxes();
        
        
    });
    $("#dialog-form-TransSettings").dialog({
        autoOpen: false,

        width: 580,
        modal: true,
        buttons: {
            "Preview": function () {
                SetObjectVisibility();
                $(this).dialog("close");
            }
        }

    });



    $("#btnSelect").click(function () {
        $("#dialog-form-SelectTrans").dialog("open");
    });
    $("#dialog-form-SelectTrans").dialog({
        autoOpen: false,

        width: 580,
        modal: true,

    });

    $("#divStudentName").click(function () {
        SetPopupProperties('#divStudentName');

    });
    $("#divHeader").click(function () {
        SetPopupProperties('#divHeader');
     
    });
    $("#divOptionalText").click(function () {
        SetPopupProperties('#divOptionalText');

    });
    $("#divStudentAddress").click(function () {
        SetPopupProperties('#divStudentAddress');

    });
    $("#divCustomField1").click(function () {
        SetPopupProperties('#divCustomField1');

    });
    $("#divCustomField2").click(function () {
        SetPopupProperties('#divCustomField2');

    });
    $("#divCustomField3").click(function () {
        SetPopupProperties('#divCustomField3');

    });
    $("#divDatePrint").click(function () {
        SetPopupProperties('#divDatePrint');

    });

    $("#coursedetails").click(function () {
        $("#dialog-form-FieldProperties").dialog("open");
    });
    $("#dialog-form").dialog({
        autoOpen: false,

        width: 580,
        modal: true,
        buttons: {
            "Preview": function () {
                

                $(globalFieldName).css("font-size", $("#txtFontSize").val() + "px")
                $(globalFieldName).css("text-align", $("#selectedAlignment").val())
                
                if (globalFieldName == "#divStudentName") {
                    $("#divStudentAddress").css("text-align", $("#selectedAlignment").val())
                }
                if (custTrans.validateCustomField()) //make sure to alert the user if the selected field should be none
                {
                    $(this).dialog("close");
                }

                if ((globalFieldName == "#divOptionalText") || (globalFieldName == "#divHeader")) {
                    
                    $(globalFieldName+"Value").text($("#optionalTextValue").val());
                }
                if ((globalFieldName == "#divCustomField1") || (globalFieldName == "#divCustomField2") || (globalFieldName == "#divCustomField3")) {
                    $(globalFieldName+"Value").text($("#selectedstudfield1").val());
                }
            },
            "Close": function () {
                    $(this).dialog("close");
             }
        }

    });



    //Save Action

    $("#Update").click(function () {
        pdfproperty = BuildjsonPdfMainProperty();
        studentname = BuildJsonProperty('divStudentName');
        header = BuildJsonProperty('divHeader');
        optionaltext = BuildJsonProperty('divOptionalText');
        studentaddress = BuildJsonProperty('divStudentAddress');
        customfield3 = BuildJsonProperty('divCustomField3');
        customfield2 = BuildJsonProperty('divCustomField2');
        customfield1 = BuildJsonProperty('divCustomField1');
        coursedetails = BuildJsonProperty('coursedetails');
        dateprint = BuildJsonProperty('divDatePrint');
        var transcriptid = $("#transid").val();
        var DefaultTranscript = $("#defaultcertTemp").val();
        var DefaultSort = $("#drpDefaultSorting").val();
        var fieldlist = {
            selectedfield: [],
            availablefield:[]
        };
        var id = "";
        $('.selectedfields').each(function () {
            result = "";
            var sortindex = 0;
            $(this).find("li").each(function () {
                id = this.id;
                if(id!=""){
                    fieldlist.selectedfield.push({
                    "id": id,
                    "sort": sortindex
                });
                sortindex = sortindex + 1;
                }
            });
            
        });

        $('.availablefields').each(function () {
            result = "";
            var sortindex = 0;
            $(this).find("li").each(function () {
                id = this.id;
                if (id != "") {
                    fieldlist.availablefield.push({
                        "id": id,
                        "sort": sortindex
                    });
                    sortindex = sortindex + 1;
                }
            });

        });


        selectedfields = JSON.stringify(fieldlist);
        $.ajax({
            type: "POST",
            url: "/Adm/CustomTranscript/UpdateTranscriptTemplate",
            data: {
                transcriptid: transcriptid,
                header: header,
                optionaltext: optionaltext,
                studentname: studentname,
                studentaddress: studentaddress,
                customfield1: customfield1,
                customfield2: customfield2,
                customfield3: customfield3,
                pdfproperty: pdfproperty,
                dateprint : dateprint,
                selectedfields: selectedfields,
                coursedetails: coursedetails,
                DefaultTranscript: DefaultTranscript,
                DefaultSort: DefaultSort

            },
            success: function (data) {
                alert(data);
                window.location.href = '/adm/CustomTranscript/TranscriptGenerator?tid=' + transcriptid
            },
        });
    });




    $('#PDFDocs').resizable({
        resize: function (event, ui) {
            
            $(this).resizable({ minWidth: ui.size.width });
            $(this).resizable({ minHeight: ui.size.height });
            
        }
    });
});

//end function
var globalFieldName = "";
function SetPopupProperties(globalVar)
{
    globalFieldName = globalVar;
    $('#ui-id-4').text($(globalVar).text());
    $("#txtFontSize").val($(globalVar).css("font-size").replace("px", ""));
    $("#selectedAlignment").val($(globalVar).css("text-align"));

    if ((globalVar == "#divCustomField1") || (globalVar == "#divCustomField2") || (globalVar == "#divCustomField3")) {
        $("#customFieldList").css("display", "block");
        var selectedFieldVal = $(globalVar).text()
        $('#selectedstudfield1').val(selectedFieldVal);
    }
    else {
        $("#customFieldList").css("display", "none")

    }
    if (globalVar == "#divStudentAddress") {
        $("#divAlign").css("display", "none");
    }
    else {
        $("#divAlign").css("display", "block");
    }
    if ((globalVar== "#divOptionalText")   || (globalFieldName == "#divHeader")) {
        $("#optionalText").css("display", "block");
        if ((globalFieldName == "#divOptionalText") || (globalFieldName == "#divHeader")) {

            $("#optionalTextValue").val($(globalVar+"Value").text());
        }
    }
    else {
        $("#optionalText").css("display", "none");
    }


    $("#dialog-form").dialog("open");
}

function BuildJsonProperty(divid) {
    var property = new Object();
    property.visible = $("#" + divid).css('visibility') == 'visible';
    property.width = $("#" + divid).width();
    property.height = $("#" + divid).height();
    property.x = $("#" + divid).css('left');
    property.y = $("#" + divid).css('top');  
    property.font = $("#" + divid).css('font-size');
    property.align = $("#" + divid).css('text-align');
    if ((divid == "divOptionalText") || divid == ("divHeader") || (divid == "divCustomField1") || (divid == "divCustomField2") || (divid == "divCustomField3")) {
        property.additionalinfo = $("#" + divid+"Value").text().replace("\n","");
    }

    if (divid == "coursedetails") {
        property.sort=$('#drpDefaultSorting').val()
    }
    
   
    var jsonString = JSON.stringify(property);
    return jsonString;

}

function BuildjsonPdfMainProperty() {
    var property = new Object();
    property.description = $("#txtCertTitle").val();
    property.width = $("#txtboundx").val();
    property.height = $("#txtboundy").val();
    property.margintop = $("#txttop").val();
    property.marginbottom = $("#txtbottom").val();
    property.marginleft = $("#txtleft").val();
    property.marginright = $("#txtright").val();
    var jsonString = JSON.stringify(property);
    return jsonString;
        
}

function GetSelectedCourseFields() {

}

function SaveCustomTranscript() {

}

function selectedindex_changed1() {
    var selected = $("#selectedLayout").val();
    var xnewWidth = $("#txtboundx").val();
    var ynewHeight = $("#txtboundy").val();

    if ((selected == 1) & (xnewWidth < ynewHeight)) {
        $("#txtboundx").val(ynewHeight);
        $("#txtboundy").val(xnewWidth);
    }

    if ((selected == 0) & (xnewWidth > ynewHeight)) {
        $("#txtboundx").val(ynewHeight);
        $("#txtboundy").val(xnewWidth);
    }
}

function SetObjectVisibility()
{
   
    if ($('#chkHeader1').prop('checked') == true) {
        $('#divHeader').css("visibility", "visible");

    } else {
        $('#divHeader').css("visibility", "hidden");
    }

    if ($('#chkOptionalText').prop('checked') == true) {
        $('#divOptionalText').css("visibility", "visible");

    } else {
        $('#divOptionalText').css("visibility", "hidden");
    }
    if ($('#chkStudentAddress').prop('checked') == true) {
        $('#divStudentAddress').css("visibility", "visible");

    } else {
        $('#divStudentAddress').css("visibility", "hidden");
    }
    if ($('#chkCustomField1').prop('checked') == true) {
        $('#divCustomField1').css("visibility", "visible");

    } else {
        $('#divCustomField1').css("visibility", "hidden");
    }
    if ($('#chkCustomField2').prop('checked') == true) {
        $('#divCustomField2').css("visibility", "visible");

    } else {
        $('#divCustomField2').css("visibility", "hidden");
    }
    if ($('#chkCustomField3').prop('checked') == true) {
        $('#divCustomField3').css("visibility", "visible");

    } else {
        $('#divCustomField3').css("visibility", "hidden");
    }
    if ($('#chkDatePrint').prop('checked') == true) {
        $('#divDatePrint').css("visibility", "visible");

    } else {
        $('#divDatePrint').css("visibility", "hidden");
    }


}

function SetCheckboxes() {
    
    if ($('#divHeader').css('visibility') == "visible") {
        $('#chkHeader1').prop('checked', 'true');
    } else {
        $('#chkHeader1').attr('checked', false);
    }



    if ($('#divOptionalText').css('visibility') == "visible") {
        $('#chkOptionalText').prop('checked', 'true');
    } else {
        $('#chkOptionalText').attr('checked', false);
    }

    if ($('#divStudentAddress').css('visibility') == "visible") {
        $('#chkStudentAddress').prop('checked', true);
    } else {
        $('#chkStudentAddress').attr('checked', false);
    }

    if ($('#divCustomField1').css('visibility') == "visible") {
        $('#chkCustomField1').prop('checked', 'true');
    } else {
        $('#chkCustomField1').attr('checked', false);
    }

    if ($('#divCustomField2').css('visibility') == "visible") {
        $('#chkCustomField2').prop('checked', 'true');
    } else {
        $('#chkCustomField2').attr('checked', false);
    }
    if ($('#divCustomField3').css('visibility') == "visible") {
        $('#chkCustomField3').prop('checked', 'true');
    } else {
        $('#chkCustomField3').attr('checked', false);
    }
    if ($('#divDatePrint').css('visibility') == "visible") {
        $('#chkDatePrint').prop('checked', 'true');
    } else {
        $('#chkDatePrint').attr('checked', false);
    }


    $("#dialog-form-TransSettings").dialog("open");
}

function CreateNewCertificate() {
    $.ajax({
        type: "POST",
        url: "/Adm/CustomTranscript/CreateNewTranscript",
        success: function (data) {
            window.location.href = '/adm/CustomTranscript/TranscriptGenerator?tid=' + data
        },
    });
}

var custTrans = {
    validateCustomField: function () {
        var clickedField = $(globalFieldName);
        if (!clickedField) return false;

        var selectedField = $('#selectedstudfield1').val();
        if (selectedField === '' && $('#selectedstudfield1').is(':visible')) {
            var response = confirm('You selected None on the Select Field. Are you sure you want to continue?');
            return response;
        }
        return true;
    }
}