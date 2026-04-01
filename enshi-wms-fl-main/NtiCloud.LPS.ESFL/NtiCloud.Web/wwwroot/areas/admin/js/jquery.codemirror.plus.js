$.extend({
    setValue: function (editorObj,value) {
        editorObj.setValue(value || "");
        editorObj.closeHint();
        setTimeout(function () {
            editorObj.refresh();
        }, 100); 
    }
});