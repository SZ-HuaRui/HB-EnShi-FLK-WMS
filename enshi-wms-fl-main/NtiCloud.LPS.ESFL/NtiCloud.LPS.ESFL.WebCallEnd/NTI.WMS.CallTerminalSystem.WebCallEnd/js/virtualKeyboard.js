var target;				//虚拟键盘操作的目标DOM
var holdOperationTime;		//setTimeout按住按键
var continuOperationTime;	//setInterval连续操作

function virtualKeyboard(targetID){
	$("#virtualKeyboard").bind("selectstart", function(){
		return false;			//阻止虚拟键盘中鼠标拖动时的选中操作
	});
	
	target = document.getElementById(targetID);		//target即为通过targetID参数传递进入的目标DOM
	$("#" + targetID).focus();						//自动将目标获取焦点
	$("#" + targetID).bind("mouseup keyup", function(){
		if (document.all){		//IE浏览器兼容
			selectRange = document.selection.createRange();	
		}						//目标上mouseup,keyup时需要获取当前的选定区域（否则当选取范围之后立即失去焦点时，本次的选取范围不会被记录）
		return false;
	});
		
	/*****************************************大小写锁定键外观、功能********************************************/
	var capsLock = "off";												//初始化大小写锁定键状态，on大写，off小写
	$("#virtualKeyboard #capsLockButton").click(function(){			//大小写锁定键功能
		if(capsLock == "on"){
			$(this).removeClass("capsLockButton_on").addClass("capsLockButton_off");
			$("#virtualKeyboard #inputButton").each(function(){
				$(this).html($(this).html().toLowerCase());  			//转换为小写键盘
				capsLock = "off";
			});
		} else if(capsLock == "off"){
			$(this).removeClass("capsLockButton_off").addClass("capsLockButton_on");
			$("#virtualKeyboard #inputButton").each(function(){
				$(this).html($(this).html().toUpperCase());			//转换为大写键盘
				capsLock = "on";
			});
		}
	});
	
	/*****************************************普通按键操作_字母&数字&符号********************************************/
	$("#virtualKeyboard #inputButton").bind("mousedown", function(){
		var inputContent = $(this).html().toString();										//获取当前输入的按键值
		singleClickInputButton(inputContent);												//触发单击操作，传递当前输入值
		holdOperationTime = setTimeout(function(){holdInputButton(inputContent)}, 700);	//长按700ms后触发连续输入操作，传递当前输入值
	}).bind("mouseup", function(){			//mouseup时解除连续输入状态
		clearTimeout(holdOperationTime);
		clearInterval(continuOperationTime);
		if (document.all){					//IE浏览器兼容
			selectRange.select();			//设置光标位置
		} else {												//非IE浏览器兼容
			target.setSelectionRange(selectRange, selectRange);	//设置并显示光标位置
		}
	}).bind("mouseout", function(){			//mouseout时解除连续输入状态
		clearTimeout(holdOperationTime);
		clearInterval(continuOperationTime);
	});

	/*****************************************退格按键操作********************************************/
	$("#virtualKeyboard #backButton").mousedown(function(){
		holdOperationTime = setTimeout(holdBackButton, 700);		//长按700ms后触发连续删除操作
		singleClickBackButton();									//触发单击操作
	}).bind("mouseup", function(){			//mouseup时解除连续输入状态
		clearTimeout(holdOperationTime);
		clearInterval(continuOperationTime);
		if (document.all){					//IE浏览器兼容
			selectRange.select();			//设置光标位置
		} else {												//非IE浏览器兼容
			target.setSelectionRange(selectRange, selectRange);	//设置并显示光标位置
		}
	}).bind("mouseout", function(){			//mouseout时解除连续输入状态
		clearTimeout(holdOperationTime);
		clearInterval(continuOperationTime);
	});
	
	/*****************************************空格键操作********************************************/
	$("#virtualKeyboard #spaceButton").mousedown(function(){
		holdOperationTime = setTimeout(holdSpaceButton, 700);		//长按700ms后触发连续删除操作
		singleClickSpaceButton();									//触发单击操作
	}).bind("mouseup", function(){			//mouseup时解除连续输入状态
		clearTimeout(holdOperationTime);
		clearInterval(continuOperationTime);
		if (document.all){					//IE浏览器兼容
			selectRange.select();			//设置光标位置
		} else {												//非IE浏览器兼容
			target.setSelectionRange(selectRange, selectRange);	//设置并显示光标位置
		}
	}).bind("mouseout", function(){			//mouseout时解除连续输入状态
		clearTimeout(holdOperationTime);
		clearInterval(continuOperationTime);
	});
}



/*****************************************普通按键单击操作_字母&数字&符号********************************************/
var selectRange;		//IE：当前选取范围or光标位置；非IE：新的光标位置
function singleClickInputButton(inputContent){
	target.focus();
	if (document.all){					//IE浏览器兼容
		try{
			selectRange.select();		//设置光标位置(获取焦点后光标会定位到开头，需要重新选中)
		} catch(e){
			selectRange = document.selection.createRange();		//获取选中文本
		}
		selectRange.text = inputContent;						//设置文本内容
    } else {							//非IE浏览器兼容
		selectRange = target.selectionStart + inputContent.length;	//计算新的光标位置（当前选择区起点+输入字符长度）
        target.value = target.value.substr(0, target.selectionStart) + inputContent + target.value.substring(target.selectionEnd);
		target.setSelectionRange(selectRange, selectRange);			//设置并显示光标位置（连续输入时不触发mouseup事件，因此在这里显示光标）
    }
}
/*****************************************普通按键按住不放操作_字母&数字&符号********************************************/
function holdInputButton(inputContent){
	continuOperationTime = setInterval(function(){singleClickInputButton(inputContent)}, 30);			//以30ms间隔连续输入
}

/*****************************************退格按键单击操作********************************************/
function singleClickBackButton(){
	target.focus();
	if (document.all){					//IE浏览器兼容
		try{
			selectRange.select();								//设置光标位置
		} catch(e){
			selectRange = document.selection.createRange();		//获取选中文本
		}
		if(selectRange.compareEndPoints("StartToStart", target.createTextRange()) == 0 && selectRange.text == ""){		//若光标在开头位置且没有选中内容
			return false;																								//则什么也不做
		} else {
			if(selectRange.text == ""){						//若没有选中内容
				selectRange.moveStart("character",-1);		//选区起点向前移动一个字符
				selectRange.select();						//选中
			}
			document.selection.clear();						//删除选中的内容
		}
    } else {							//非IE浏览器兼容
		if(target.selectionStart != target.selectionEnd){	//选中内容时（此时只需删除选区内的内容）
			selectRange = target.selectionStart				//计算新的光标位置（当前选择区起点）
			target.value = target.value.substr(0, target.selectionStart) + target.value.substring(target.selectionEnd);
		} else {											//未选中内容时（此时需要删除光标之前的一个字符）
			selectRange = target.selectionStart - 1;		//计算新的光标位置（当前选择区起点 - 1个字符）
			target.value = target.value.substr(0, target.selectionStart-1) + target.value.substring(target.selectionEnd);
		}
		target.setSelectionRange(selectRange, selectRange);		//设置并显示光标位置（连续输入时不触发mouseup事件，因此在这里显示光标）
	}
}
/*****************************************退格按键按住不放操作********************************************/
function holdBackButton(){
	continuOperationTime = setInterval(singleClickBackButton, 30);			//以30ms间隔连续删除
}

/*****************************************空格键单击操作********************************************/
var selectRange;		//IE：当前选取范围or光标位置；非IE：新的光标位置
function singleClickSpaceButton(){
	target.focus();
	if (document.all){					//IE浏览器兼容
		try{
			selectRange.select();		//设置光标位置(获取焦点后光标会定位到开头，需要重新选中)
		} catch(e){
			selectRange = document.selection.createRange();		//获取选中文本
		}
		selectRange.text = " ";						//设置文本内容
    } else {							//非IE浏览器兼容
		selectRange = target.selectionStart + 1;					//计算新的光标位置（当前选择区起点+1）
        target.value = target.value.substr(0, target.selectionStart) + " " + target.value.substring(target.selectionEnd);
		target.setSelectionRange(selectRange, selectRange);			//设置并显示光标位置（连续输入时不触发mouseup事件，因此在这里显示光标）
    }
}
/*****************************************空格键按住不放操作********************************************/
function holdSpaceButton(inputContent){
	continuOperationTime = setInterval(singleClickSpaceButton, 30);			//以30ms间隔连续输入
}



  /* ****************************************************************
   * Insert text at the cursor
   *
   */
   /*
  this.VKI_insert = function(text) {
    this.VKI_target.focus();
    if (this.VKI_target.maxLength) this.VKI_target.maxlength = this.VKI_target.maxLength;
    if (typeof this.VKI_target.maxlength == "undefined" || this.VKI_target.maxlength < 0 || this.VKI_target.value.length < this.VKI_target.maxlength) {
      if (this.VKI_target.setSelectionRange && !this.VKI_target.readOnly && !this.VKI_isIE) {
        var rng = [this.VKI_target.selectionStart, this.VKI_target.selectionEnd];
        this.VKI_target.value = this.VKI_target.value.substr(0, rng[0]) + text + this.VKI_target.value.substr(rng[1]);
        if (text == "\n" && this.VKI_isOpera) rng[0]++;
        this.VKI_target.setSelectionRange(rng[0] + text.length, rng[0] + text.length);
      } else if (this.VKI_target.createTextRange && !this.VKI_target.readOnly) {
        try {
          this.VKI_target.range.select();
        } catch(e) {
		  this.VKI_target.range = document.selection.createRange(); 
		}
        this.VKI_target.range.text = text;
        this.VKI_target.range.collapse(true);
        this.VKI_target.range.select();
      } else this.VKI_target.value += text;
      if (this.VKI_shift) this.VKI_modify("Shift");
      if (this.VKI_altgr) this.VKI_modify("AltGr");
      this.VKI_target.focus();
    } else if (this.VKI_target.createTextRange && this.VKI_target.range)
      this.VKI_target.range.select();
  };
  
  */