$(function () {
    var isPK = [{ "Name": "是", "Value": "是" }, { "Name": "否", "Value": "否" }], fieldType = [];
    if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
        fieldType = [
            {
                "Name": "字符串",
                "Value": "VARCHAR2"
            },
            {
                "Name": "N字符串",
                "Value": "NVARCHAR2"
            },
            {
                "Name": "超长字符串",
                "Value": "CLOB"
            },
            {
                "Name": "布尔型",
                "Value": "CHAR"
            },
            {
                "Name": "日期",
                "Value": "DATE"
            },
            {
                "Name": "数字型",
                "Value": "NUMBER"
            }
        ];
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "MySql") {
        fieldType = [
            {
                "Name": "字符串",
                "Value": "varchar"
            },
            {
                "Name": "N字符串",
                "Value": "text"
            },
            {
                "Name": "超长字符串",
                "Value": "longtext"
            },
            {
                "Name": "字符型",
                "Value": "char"
            },
            {
                "Name": "布尔型",
                "Value": "bit"
            },
            {
                "Name": "日期",
                "Value": "datetime"
            },
            {
                "Name": "数字型",
                "Value": "decimal"
            },
            {
                "Name": "整型",
                "Value": "int"
            }
        ];
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "PostgreSql") {
        fieldType = [
            {
                "Name": "字符串",
                "Value": "varchar"
            },
            {
                "Name": "N字符串",
                "Value": "text"
            },
            {
                "Name": "超长字符串",
                "Value": "text"
            },
            {
                "Name": "字符型",
                "Value": "bpchar"
            },
            {
                "Name": "布尔型",
                "Value": "bool"
            },
            {
                "Name": "日期",
                "Value": "timestamp"
            },
            {
                "Name": "数字型",
                "Value": "numeric"
            },
            {
                "Name": "整型",
                "Value": "int4"
            }
        ];
    }
    else {
        fieldType = [
            {
                "Name": "字符串",
                "Value": "varchar"
            },
            {
                "Name": "N字符串",
                "Value": "nvarchar"
            },
            {
                "Name": "超长字符串",
                "Value": "nvarchar(Max)"
            },
            {
                "Name": "字符型",
                "Value": "char"
            },
            {
                "Name": "布尔型",
                "Value": "bit"
            },
            {
                "Name": "日期",
                "Value": "datetime"
            },
            {
                "Name": "数字型",
                "Value": "decimal"
            },
            {
                "Name": "整型",
                "Value": "int"
            }
        ];
    }
    var isNull = [{ "Name": "可为空", "Value": "Y" }, { "Name": "不可为空", "Value": "N" }];

    $("#tableName").textbox("setValue", common.getUrlParam("id"));
    var btdatagrid = $("#List").datagrid({
        loader: function (param, success, error) {
            common.postGateway('platform/table/tablefield', {
                "tableName": (common.getUrlParam("id") == null ? "" : common.getUrlParam("id"))
            }, common.getToken(), function (tableData) {
                success(JSON.parse(tableData.json));
            });
        },
        width: SetGridWidthSub(10),
        height: SetGridHeightSub(115),
        title: "表结构" + JQueryEasyUI.gridToolsMenu("#List", 'defaultTable'),
        nowrap: true,
        striped: true,
        fitColumns: true,
        remoteSort: false,
        animate: true,
        singleSelect: true,
        rownumbers: true,//行号
        columns: [[
            {
                field: 'fldkey', title: '主键', width: 50, align: 'center', editor: {
                    type: 'combobox',
                    options: {
                        data: isPK,
                        panelHeight: 'auto',
                        panelMaxHeight: 250,
                        valueField: 'Value',
                        textField: 'Name',
                        editable: false
                    }
                }
            },
            { field: 'fldname', title: '字段名称', width: 100, align: 'left', halign: 'center', editor: { type: "textbox", options: { required: true } } },
            { field: 'fldcomments', title: '字段描述', width: 100, align: 'left', halign: 'center', editor: "textbox" },
            {
                field: 'fldtype', title: '字段类型', width: 65, align: 'center', editor: {
                    type: 'combobox',
                    options: {
                        panelHeight: 'auto',
                        panelMaxHeight: 250,
                        data: fieldType,
                        valueField: 'Value',
                        textField: 'Name',
                        editable: false,
                        required: true
                    }
                }
            },
            { field: 'fldlength', title: '字段长度', width: 50, align: 'center', editor: "numberbox" },
            { field: 'fldprecision', title: '精度', width: 60, align: 'center', editor: "numberbox" },
            { field: 'fldvalue', title: '默认值', width: 65, align: 'center', editor: "textbox" },
            {
                field: 'fldnull', title: '允许为空', width: 50, align: 'center', editor: {
                    type: 'combobox',
                    options: {
                        data: isNull,
                        panelHeight: 'auto',
                        panelMaxHeight: 250,
                        valueField: 'Value',
                        textField: 'Name',
                        editable: false
                    }
                }
            }
        ]],
        onLoadSuccess: function (data) {
            $("#tableComments").textbox("setValue", data.note);
        },
        onClickRow: function (rowData) {
            for (var n = 0; n < btdatagrid.datagrid("getData").rows.length; n++) {
                btdatagrid.datagrid("endEdit", n);
            }
        },
        onDblClickRow: function (rowIndex, rowData) {
            for (var n = btdatagrid.datagrid("getData").rows.length - 1; n >= 0; n--) {
                btdatagrid.datagrid("endEdit", n);
            }

            var rows = btdatagrid.datagrid('getEditingRowIndexs');
            if (rows.length == 0) {//单击开启编辑行
                btdatagrid.datagrid("beginEdit", rowIndex);
            }
        }
    });

    $("#btnSave").click(function () {
        var tableName = $("#tableName").textbox("getValue");
        var tablePrefix = $("#tablePrefix").val();
        if (tableName == "" || tableName == null) {
            $.messageBoxAlert(0, '输入表名为空，请重新输入！', function () {
                $("#tableName").textbox("textbox").focus();
                $.messageBoxCloseLoading();
            });
            return;
        }
        else if (tableName.toUpperCase().indexOf(tablePrefix) != 0) {
            $.messageBoxAlert(0, '表名命名非法，请重新输入！', function () {
                $("#tableName").textbox("textbox").focus();
                $.messageBoxCloseLoading();
            });
            return;
        }
        var name = tableName.replace(/(^\s*)|(\s*$)/g, "").toUpperCase();
        validate(name);
    });

    $("#btnSaveClose").click(function () {
        var tableName = $("#tableName").textbox("getValue");
        if (tableName == "" || tableName == null) {
            $.messageBoxAlert(0, '输入表名为空，请重新输入！', function () {
                $("#tableName").textbox("textbox").focus();
                $.messageBoxCloseLoading();
            });
            return;
        }
        var name = tableName.replace(/(^\s*)|(\s*$)/g, "").toUpperCase();
        $("#createSql").val(createTableStatement(name));
        $.messageBoxOpenID("<span class='mdi mdi-content-save'></span>生成建表SQL", ['400px', '380px'], "#openList");
    });

    $("#btnReturn").click(function () {
        window.parent.frameReturnByClose();
    });
});

function execSQL(sql) {
    apiRoute.runTaskEvent("",sql, [], function (data) {
        if (data.resMsg != '') {
            $.messageBoxAlert(0, data.resMsg);
        }
        else {
            $.messageBoxAlert(1, '数据表创建成功！', function () {
                window.parent.frameReturnByReload(false);
            });
        }
    });
}

function validate(name) {
    common.postGateway('platform/table/DbTable', {
        "tableName": name
    }, common.getToken(), function (data) {
        var sqlString = createTableStatement(name);
        if (data.json == "Yes") {
            $.messageBoxConfirm('重新创建此表将删除表中所有数据!<br>是否重新创建此表？', ['重建此表', '取消关闭'], function () {
                execSQL("DROP TABLE " + name + "; " + sqlString.replace(/'\r\n'/g, ' '));
            });
        }
        else {
            execSQL(sqlString.replace(/'\r\n'/g, ' '));
        }
    });
}


function createTableStatement(tableName) {
    for (var n = 0; n < $("#List").datagrid("getData").rows.length; n++) {
        $("#List").datagrid("endEdit", n);
    }
    var name = tableName;
    var rows = $("#List").datagrid("getRows");
    for (var i = 0; i < rows.length; i++) {
        rows[i]['fldname'] = rows[i]['fldname'].toUpperCase();
    }
    var statement = 'CREATE TABLE ' + name + ' \r\n( ';
    if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "Oracle") {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldtype'] == 'CHAR') {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype'] + '(1)';
            }
            else if (rows[i]['fldtype'] == 'NUMBER' && rows[i].fldprecision > 0) {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ','
                    + rows[i].fldprecision
                    + ')'
            }
            else if (rows[i]['fldlength'] > 0 && rows[i]['fldtype'] != 'DATE' && rows[i]['fldtype'] != 'CLOB') {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ')';
            }
            else {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype'];
            }
            if (rows[i]['fldvalue'] != '' && rows[i]['fldvalue'] != null) {
                statement += ' ' + 'DEFAULT ' + ' \'' + rows[i]['fldvalue'] + '\' '
            }
            if (rows[i]['fldnull'] == 'N') {
                statement += " NOT NULL"
            }
            statement += " ,\r\n";
        }

        statement = statement.substring(0, statement.lastIndexOf(",") > 0 ? statement.lastIndexOf(",") : statement.Length) + " )";
        //statement += 'TABLESPACE NTI PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE (INITIAL 64K NEXT 8K MINEXTENTS 1 MAXEXTENTS UNLIMITED)';
        statement += ';\r\n';
        //添加主键
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldkey'] == '是') {
                statement += 'ALTER TABLE ' + name + ' ADD CONSTRAINT ' + 'PK_' + name + ' ' + 'PRIMARY KEY' + '(' + rows[i]['fldname'] + ');\r\n'
            }
        }

        statement += 'COMMENT ON TABLE ' + name + ' IS ' + '\'' + $("#tableComments").textbox('getValue') + '\'' + ';\r\n';

        //添加注释
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].fldcomments != null && rows[i].fldcomments != '') {
                if (i != rows.length - 1) {
                    statement += 'COMMENT ON COLUMN ' + name + '.' + rows[i]['fldname'] + ' IS ' + '\'' + rows[i].fldcomments + '\'' + ';\r\n';
                }
                else {
                    statement += 'COMMENT ON COLUMN ' + name + '.' + rows[i]['fldname'] + ' IS ' + '\'' + rows[i].fldcomments + '\'';
                }
            }
        }
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "MySql") {
        statement = 'CREATE TABLE `' + name + '` \r\n( ';
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldtype'] == 'decimal' && rows[i].fldprecision > 0) {
                statement += '`' + rows[i]['fldname'] + '`'
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ','
                    + rows[i].fldprecision
                    + ')'
            }
            else if (rows[i]['fldlength'] > 0 && (rows[i]['fldtype'] == 'varchar' || rows[i]['fldtype'] == 'char')) {
                statement += '`' + rows[i]['fldname'] + '`'
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ')';
            }
            else if (rows[i]['fldtype'] == 'datetime') {
                statement += '`' + rows[i]['fldname'] + '`'
                    + ' '
                    + rows[i]['fldtype']
                    + '(0)';
            }
            else {
                statement += '`' + rows[i]['fldname'] + '`'
                    + ' '
                    + rows[i]['fldtype'];
            }
            if (rows[i]['fldtype'] != 'datetime' && rows[i]['fldtype'] != 'bit' && rows[i]['fldtype'] != 'int' && rows[i]['fldtype'] != 'decimal') {
                statement += " CHARACTER SET utf8 COLLATE utf8_bin";
            }
            if (rows[i]['fldnull'] == 'N') {
                statement += " NOT NULL";
            }
            else {
                statement += " NULL";
            }
            if (rows[i]['fldvalue'] != '' && rows[i]['fldvalue'] != null) {
                statement += ' ' + 'DEFAULT ' + ' \'' + rows[i]['fldvalue'] + '\' ';
            }
            if (rows[i].fldcomments != null && rows[i].fldcomments != '') {
                statement += ' ' + ' COMMENT ' + ' \'' + rows[i]['fldcomments'] + '\' ';
            }
            statement += " ,\r\n";
        }
        //添加主键
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldkey'] == '是') {
                statement += 'PRIMARY KEY (`' + rows[i]['fldname'] + '`) USING BTREE\r\n'
            }
        }
        statement += ') ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_bin COMMENT = \'' + $("#tableComments").textbox('getValue') + '\' ROW_FORMAT = Dynamic';
    }
    else if (JSON.parse(common.Decrypt(sessionStorage["encrypt"])).UserDbConnR.dbType == "PostgreSql") {
        name = name.toLowerCase();
        statement = 'CREATE TABLE "public"."' + name + '" \r\n( ';
        for (var i = 0; i < rows.length; i++) {
            rows[i]['fldname'] = rows[i]['fldname'].toLowerCase();
            if (rows[i]['fldtype'] == 'numeric' && rows[i].fldprecision > 0) {
                statement += '"' + rows[i]['fldname'] + '"'
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ','
                    + rows[i].fldprecision
                    + ')'
            }
            else if (rows[i]['fldlength'] > 0 && (rows[i]['fldtype'] == 'varchar' || rows[i]['fldtype'] == 'char')) {
                statement += '"' + rows[i]['fldname'] + '"'
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ')';
            }
            else if (rows[i]['fldlength'] > 0 && rows[i]['fldtype'] == 'bpchar') {
                statement += '"' + rows[i]['fldname'] + '"'
                    + ' '
                    + 'char('
                    + rows[i]['fldlength']
                    + ')';
            }
            else if (rows[i]['fldtype'] == 'timestamp') {
                statement += '"' + rows[i]['fldname'] + '"'
                    + ' '
                    + rows[i]['fldtype']
                    + '(6)';
            }
            else {
                statement += '"' + rows[i]['fldname'] + '"'
                    + ' '
                    + rows[i]['fldtype'];
            }
            if (rows[i]['fldtype'] != 'timestamp' && rows[i]['fldtype'] != 'bool' && rows[i]['fldtype'] != 'int4' && rows[i]['fldtype'] != 'numeric') {
                statement += ' COLLATE "pg_catalog"."default" ';
            }
            if (rows[i]['fldnull'] == 'N') {
                statement += " NOT NULL";
            }
            if (rows[i]['fldvalue'] != '' && rows[i]['fldvalue'] != null) {
                statement += ' ' + 'DEFAULT ' + ' \'' + rows[i]['fldvalue'] + '\' ::text';
            }
            statement += " ,\r\n";
        }
        statement = statement.substring(0, statement.lastIndexOf(",") > 0 ? statement.lastIndexOf(",") : statement.Length) + " )";
        statement += ';\r\n';
        //添加注释
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].fldcomments != null && rows[i].fldcomments != '') {
                if (i != rows.length - 1) {
                    statement += 'COMMENT ON COLUMN "public"."' + name + '"."' + rows[i]['fldname'] + '" IS ' + '\'' + rows[i].fldcomments + '\'' + ';\r\n';
                }
                else {
                    statement += 'COMMENT ON COLUMN "public"."' + name + '"."' + rows[i]['fldname'] + '" IS ' + '\'' + rows[i].fldcomments + '\';\r\n';
                }
            }
        }
        statement += 'COMMENT ON TABLE "public"."' + name + '" IS ' + '\'' + $("#tableComments").textbox('getValue') + '\'' + ';\r\n';

        //添加主键
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldkey'] == '是') {
                statement += 'ALTER TABLE "public"."' + name + '"  ADD CONSTRAINT "' + name +'_pkey" PRIMARY KEY ("' + rows[i]['fldname'] + '") \r\n'
            }
        }
    }
    else {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldtype'] == 'decimal' && rows[i].fldprecision > 0) {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ','
                    + rows[i].fldprecision
                    + ')'
            }
            else if (rows[i]['fldlength'] > 0 && (rows[i]['fldtype'] == 'varchar' || rows[i]['fldtype'] == 'nvarchar' || rows[i]['fldtype'] == 'char')) {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype']
                    + '('
                    + rows[i]['fldlength']
                    + ')';
            }
            else {
                statement += rows[i]['fldname']
                    + ' '
                    + rows[i]['fldtype'];
            }

            if (rows[i]['fldnull'] == 'N') {
                statement += " NOT NULL"
            }
            else {
                statement += " NULL"
            }
            statement += " ,\r\n";
        }

        //添加主键
        var primaryKey = "";
        for (var i = 0; i < rows.length; i++) {
            if (rows[i]['fldkey'] == '是') {
                primaryKey += rows[i]['fldname'] + " ASC,"
            }
        }
        if (primaryKey == "") {
            statement = statement.substring(0, statement.lastIndexOf(",") > 0 ? statement.lastIndexOf(",") : statement.Length) + " )";
            statement += ';\r\n';
        }
        else {
            primaryKey = primaryKey.substring(0, primaryKey.lastIndexOf(",") > 0 ? primaryKey.lastIndexOf(",") : primaryKey.Length);
            statement += " CONSTRAINT [PK_" + name + "] PRIMARY KEY CLUSTERED (" + primaryKey + ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] "
            statement += " ) ON [PRIMARY];\r\n"
        }

        statement += "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + $("#tableComments").textbox('getValue') + "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'" + name + "';\r\n"

        //添加注释
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].fldcomments != null && rows[i].fldcomments != '') {
                statement += "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + rows[i].fldcomments + "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'" + name + "', @level2type=N'COLUMN',@level2name=N'" + rows[i]['fldname'] + "';\r\n";
            }
        }
    }
    return statement;
}