var formattributeJSON = { hasEditor: "0" };
var formsubtabs = [];
var formEvents = [];
var myUE = null;

var formid;

$(function () {
    formid = $("#formId").val(); //common.getUrlParam("formId");
    //实例化编辑器
    //建议使用工厂方法getEditor创建和引用编辑器实例，如果在某个闭包下引用该编辑器，直接调用UE.getEditor('editor')就能拿到相关的实例
    resize();
});

function resize() {
    $("#editor").height($(window).height() - 130);
    $("#editor").width($(window).width() - 20);
    myUE = UE.getEditor('editor', { elementPathEnabled: false, wordCount: false, maximumWords: 1000000000, autoHeightEnabled: false });
    myUE.addListener('ready', function (editor) {
        if (formid != null && formid.length > 0) {
            openform(formid);
        }
    });
}
function execCommend(method) {
    myUE.execCommand(method);
}
function openform(id) {
    apiRoute.selectDataEvent("select * from prod_app_module where t_id='" + id + "'", [],"1=1", function (result) {
        formattributeJSON.id = result[0].t_id;
        formattributeJSON.pageName = result[0].modulename;
        if (result[0].modulejson == "") return;
        formattributeJSON.addService = JSON.parse(result[0].modulejson).addService;
        formattributeJSON.editService = JSON.parse(result[0].modulejson).editService;
        formattributeJSON.pageService = JSON.parse(result[0].modulejson).pageService;
        formattributeJSON.pageIDField = JSON.parse(result[0].modulejson).pageIDField;
        myUE.setContent(result[0].modulejscript, false);
        formattributeJSON.tableName = JSON.parse(result[0].modulejson).tableName;
        formattributeJSON.fields = JSON.parse(result[0].modulejson).fields;
        formsubtabs = JSON.parse(result[0].modulejson).formsubtabs;
        formEvents = JSON.parse(result[0].modulejson).formEvents;
    });
}