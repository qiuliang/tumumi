var xmlHttp;
var returnMessage;
function createXmlHttp() {
	if (window.XMLHttpRequest) {

				//针对firefox,mozillar,opera,safari,IE7,IE8
		xmlHttp = new XMLHttpRequest();

				//针对某些特定版本的mozillar浏览器的bug进行修正
		if (xmlHttp.overrideMimeType) {
			xmlHttp.overrideMimeType("text/xml");
		}
	} else {
		if (window.ActiveXObject) {

				//针对IE6,IE5.5,IE5
			try {
				xmlHttp = new ActiveXObject("MSXML2.XMLHTTP");
			}
			catch (e) {
				try {
					xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
				}
				catch (e) {
					alert("不能创建XmlHttpRequest");
				}
			}
		}
	}
}



function sendAjaxRequest(method,url,flag,param,writeMessage){
	createXmlHttp();
	xmlHttp.open(method,url,flag);
	xmlHttp.onreadystatechange = function(){
		if(xmlHttp.readyState == 4){
				if(xmlHttp.status == 200){
					returnMessage = xmlHttp.responseText;
					eval("var flag="+returnMessage);
					emailflag = flag;
				}else if(xmlHttp.status == 404){
					returnMessage = "路径不正确 请检查路径";
				}else if(xmlHttp.status == 500){
					returnMessage = "服务器内部存在 再次检查";
				}
				writeMessage();
			}
	};
	if(method == "post"){
		xmlHttp.setRequestHeader("Content-Type","application/x-www-form-urlencoded");
	}
	xmlHttp.send(param);
	}
	
	
	function doTime(h , m , s , from) { 
    	var _t; 
	    var _h = h; 
	    var _m = m; 
	    var _s = s; 
	    var _v = '距离免费结束还有：';
	    if(from == 'index_after'){
	    	_v = "免费结束：";
	    }
	    if(from == 'index'){
	    	_v += "<br />";
	    } 
	    _v += _h + '小时' + _m + '分' + _s + '秒';
    	_s --; 
        if (_s == -1) { 
             _m --; 
             _s = 59; 
        } 
        if (_m == -1) { 
             _h --; 
             _m = 59; 
        } 
        var _b = ((_h == 0) && (_m == 0) && (_s == 0)); 
        if (_b) {
        	if(from != 'index'){
            	document.getElementById("end").style.display='none';
        	}else{
        		_v = '该文档免费活动已结束';
        	}
        	ajax();
        } else { 
            _v = '距离免费结束还有：';
            if(from == 'index_after'){
		    	_v = "免费结束：";
		    }
            if(from == 'index'){
		    	_v += "<br />";
		    } 
            _v += _h + '小时' + _m + '分' + _s + '秒'; 
            _t = setTimeout('doTime(' + _h + ' , ' + _m + ' , ' + _s + ' , \'' + from + '\')', 1000); 
        }
        document.getElementById("oTime").innerHTML = _v; 
    }
    
function startTime(h , m , s , from){
	var _v = '距离免费结束还有：';
	if(from == 'index_after'){
    	_v = "免费结束：";
    }
    if(from == 'index'){
    	_v += "<br />";
    } 
    _v += h + '小时' + m + '分' + s + '秒';
	document.getElementById("oTime").innerHTML = _v; 
	var _t = setTimeout('doTime('+ h +' , '+ m +' , '+ s +' , \'' + from + '\')', 1000); 
}
    
 function ajax(){
 	var url = "/app/user/ajax?recommendId=" + freecmdId;
	jQuery("#a5").load(url);
	
			
	//sendAjaxRequest('get',url,true,null,writeEmail);
	
}
	
	
