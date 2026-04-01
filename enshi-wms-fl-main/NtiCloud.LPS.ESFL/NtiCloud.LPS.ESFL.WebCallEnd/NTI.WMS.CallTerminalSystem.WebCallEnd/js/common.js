// JavaScript Document
$(document).ready(function () {
    $(".modifypwd").hide();
    $('#number_input').hide();
    $('#number_input_p').hide();
    $("#btn_Change_Class").click(function () {
      
        $(".modifypwd").dialog({
            resizable: false,
            width: 410,
            modal: true
        });
    });
    $('.number_bg li').bind({
        mousedown: function () {
            $(this).css('background', 'url(../images/number/btn_disabel_light.png) no-repeat');
        },
        mouseup: function () {
            $(this).css('background', 'url(../images/number/btn_disabel.png) no-repeat');
            var innerT = $(this).html().toString().replace(/\s+/g, "");
            numberString = numberOverInput.value.replace(/\s+/g, "");
            var numberString_ = numberString.toString().replace(/\s+/g, "");
            if (innerT == "←") {
                numberString_ = numberString_.substr(0, numberString_.length - 1);
                numberOverInput.value = numberString_;
            } else if (innerT == "确认") {
                numberGetInput.value = numberOverInput.value;
                b = numberGetInput.value;
                b = (b + '').replace(/^0+\./g, '0.'); b.match(/^0+[1-9]+/) ? b = b.replace(/^0+/g, '') : b; numberGetInput.value = Number(b) ? b : 0;
               // numberGetInput.value = numberOverInput.value;
                hideNumberInput();
            } else if (innerT == "关闭") {
                hideNumberInput();

            } else {
                numberString += innerT;
                numberOverInput.value = numberString;
            }
        }
    });
    $('.number_bg_p li').bind({
        mousedown: function () {
            $(this).css('background', 'url(images/number/btn_disabel_light.png) no-repeat');
        },
        mouseup: function () {
            $(this).css('background', 'url(images/number/btn_disabel.png) no-repeat');
            var innerT = $(this).html().toString().replace(/\s+/g, "");
            numberString = numberOverInput.value.replace(/\s+/g, "");
            var numberString_ = numberString.toString().replace(/\s+/g, "");
            if (innerT == "←") {
                numberString_ = numberString_.substr(0, numberString_.length - 1);
                numberOverInput.value = numberString_;
            } else if (innerT == "确认") {
                numberGetInput.value = numberOverInput.value;
                // numberGetInput.value = numberOverInput.value;
                hideNumberInput_p();
            } else if (innerT == "关闭") {
                hideNumberInput_p();

            } else {
                numberString += innerT;
                numberOverInput.value = numberString;
            }
        }
    });

});

function showNumberInput(input) {
    numberOverInput = document.getElementById('number_text_in');
    //numberOverInput = input;
    numberGetInput = input;
    numberOverInput.value = input.value;
    $('#number_input').show();

}
function hideNumberInput() {
    numberOverInput = "";
    numberString = "";

    $('#number_input').hide();
}

function showNumberInput_p(input) {
    numberOverInput = document.getElementById('number_text_in_p');
    //numberOverInput = input;
    numberGetInput = input;
    numberOverInput.value = input.value;
    $('#number_input_p').show();

}
function hideNumberInput_p() {
    numberOverInput = "";
    numberString = "";
    $('#number_input_p').hide();
}