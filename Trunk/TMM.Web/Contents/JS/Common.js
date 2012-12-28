/********************
公用方法
更新日期：2010-9-26
*********************/

/********************
序列化JSON对象
*********************/
function json2string(strObject) {   
  var c, i, l, s = '', v, p;    
  switch (typeof strObject) {
     case 'object':   
       if (strObject) {
          if (strObject.length && typeof strObject.length == 'number') {
             for (i = 0; i < strObject.length; ++i) {
                     v = json2string(strObject[i]);
                     if (s) {
                         s += ',';
                     }
                     s += v;       
              }   
              return '[' + s + ']';      
        } 
        else if (typeof strObject.toString != 'undefined') {
               for (i in strObject) {        
                 v = strObject[i];   
                 if (typeof v != 'undefined' && typeof v != 'function') {
                        v = json2string(v);   
                        if (s) {
                           s += ',';         
                        }
                        s += json2string(i) + ':' + v;        
                 }       
               }
               return '{' + s + '}';
         }     
       }   
       return 'null';   
                     case 'number':   
                     return isFinite(strObject) ? String(strObject) : 'null';
                     case 'string':     l = strObject.length; 
                         s = '"';   
                         for (i = 0; i < l; i += 1) {
                               c = strObject.charAt(i);   
                               if (c >= ' ') {   
                               if (c == '\\' || c == '"') {
                                       s += '\\';       }       
                                       s += c;      }
                                        else {
                                           switch (c) {
                                              case '\b':
                                                       s += '\\b';   break;   
                                                       case '\f':         
                                                       s += '\\f';   break;   
                                                       case '\n':         
                                                       s += '\\n';   break;   
                                                       case '\r':         
                                                       s += '\\r';   break;   
                                                       case '\t':         s += '\\t';   break;   
                                                       default:         c = c.charCodeAt();         
                                                       s += '\\u00' + Math.floor(c / 16).toString(16) +          (c % 16).toString(16);       
                                                       }      }     }   
                                                       return s + '"';   
                                                       case 'boolean':   return String(strObject);   
                                                       default:   return 'null';   
                                                       }   } 
/********************
拷贝到剪贴板
*********************/
function copyToClipboard(txt) {    
     if(window.clipboardData) {    
             window.clipboardData.clearData();    
             window.clipboardData.setData("Text", txt);    
     } else if(navigator.userAgent.indexOf("Opera") != -1) {    
          window.location = txt;    
     } else if (window.netscape) {    
          try {    
               netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");    
          } catch (e) {    
               alert("被浏览器拒绝！\n请在浏览器地址栏输入'about:config'并回车\n然后将'signed.applets.codebase_principal_support'设置为'true'");    
          }    
          
     }
     else if( window.google && window.chrome ){
        alert("浏览器不支持此操作，请手工复制");
     }
 }

 /********************
 字符串处理
 *********************/
 //去除首尾空格
 String.prototype.trim = function() {
     return this.replace(/(^\s*)|(\s*$)/g, "");
 }
 //计算字符串的长度
 String.prototype.realLen = function() {
     return this.replace(/[^\x00-\xff]/ig, "**").length;
 }
 //整除
 function zhengChu(exp1,exp2) {
     var n1 = Math.round(exp1); //四舍五入   
     var n2 = Math.round(exp2); //四舍五入   

     var rslt = n1 / n2; //除   

     if (rslt >= 0) {
         rslt = Math.floor(rslt); //返回小于等于原rslt的最大整数。   
     }
     else {
         rslt = Math.ceil(rslt); //返回大于等于原rslt的最小整数。   
     }

     return rslt;
 }   

 //jquery ajax 通用回调
 function ajaxCallback(data,callBack) {
     if (data == "1") {
         if (callBack) {
             callBack();
         }
         else {
             alert("操作成功！");
         }
     }
     else {
         alert("操作失败，请重试");
     }
 }

 //页面跳转
 function gotoPage(formName, obj) {
     $("#" + formName).append("<input type='hidden' name='first' value='" + $(obj).val() + "' />");
     //$("#" + formName).append("<input type='hidden' name='gotoPage' value='" + $(obj).val() + "' />");
     $("#" + formName).submit();
 }