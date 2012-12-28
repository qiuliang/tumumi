
function fileQueueError(file, errorCode, message) {
	try {
		var imageName = "error.gif";
		if (errorCode === SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED)
		{
			alert("您一次上传的文件不能超过"+message+"张.");
			return;
		}

		switch (errorCode)
		{
			case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
				imageName = "zerobyte.gif";
				break;
			case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
				imageName = "toobig.gif";
				break;
			case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
			case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
			default:
				alert(message);
				break;
		}

		addImage("images/" + imageName);

	} catch (ex) {
		this.debug(ex);
	}

}

function fileDialogComplete(numFilesSelected, numFilesQueued)
{
	if (numFilesQueued > 0) 
	{
		try
		{
	    var params = {};
//			params["ASPSESSID"] =document.getElementById("ASPSESSID").value;
//			params["userId"] = document.getElementById("userId").value;
//			params["albumId"] = document.getElementById("albumId").value;
//			
//			upload.setPostParams(params);
			upload.startUpload();
		}
		catch (ex) {
		    alert(ex);
		}
	}
}


function uploadProgress(file, bytesLoaded) {

	try {
		var percent = Math.ceil((bytesLoaded / file.size) * 100);

		var progress = new FileProgress(file,  this.customSettings.upload_target);
		
		progress.setProgress(percent);
		if (percent === 100)
		{
			progress.setStatus("正在保存文件...");
			progress.toggleCancel(false, this);
		} else {
			progress.setStatus("上传中...");
			progress.toggleCancel(true, this);
		}
	} catch (ex) {
		this.debug(ex);
	}
}

function uploadSuccess(file, serverData) {
     
     
	 try {
	        eval("var ro= " + serverData);
		    
		    if(ro.code == -1)
		    {
		        var progress = new FileProgress(file, this.customSettings.upload_target);
		        //progress.setError();
		        progress.setStatus("系统错误");
		        progress.toggleCancel(false);
		    }
		    else if(ro.code == 0)
		    {
		        var progress = new FileProgress(file, this.customSettings.upload_target);
		        progress.setError();
		        progress.setStatus(ro.msg);
		        progress.toggleCancel(false);
		    }
		    else if (ro.code == 1)
		    {
		        var progress = new FileProgress(file, this.customSettings.upload_target);
		        progress.setComplete();
		        progress.setStatus("上传完成.");
		        progress.toggleCancel(false);
		        document.getElementById("fileId").value = ro.msg;
		        document.getElementById("btnSubmit").click();
		        
		    }
	    } catch (ex) {
		    this.debug(ex);
	    }

	
}

function uploadComplete(file) {
	var photoIdObj=document.getElementById("photoIds");
	var albumIdObj=document.getElementById("albumId");
	try {
	    /*  I want the next upload to continue automatically so I'll call startUpload here */
		if (this.getStats().files_queued > 0) {
			this.startUpload();
		} else {

			 
		}
	} catch (ex) {
		this.debug(ex);
	}
}
/*
function uploadError(file, errorCode, message) {
	var imageName =  "error.gif";
	var progress;
	try {
		switch (errorCode) {
		case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
			try {
				progress = new FileProgress(file,  this.customSettings.upload_target);
				progress.setCancelled();
				progress.setStatus("Cancelled");
				progress.toggleCancel(false);
			}
			catch (ex1) {
				this.debug(ex1);
			}
			break;
		case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
			try {
				progress = new FileProgress(file,  this.customSettings.upload_target);
				progress.setCancelled();
				progress.setStatus("Stopped");
				progress.toggleCancel(true);
			}
			catch (ex2) {
				this.debug(ex2);
			}
		case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
			imageName = "uploadlimit.gif";
			break;
		default:
			alert("错误信息（代号）："+errorCode+"-"+message);
			break;
		}

		addImage("images/" + imageName);

	} catch (ex3) {
		this.debug(ex3);
	}

}
*/
function uploadError(file, errorCode, message) {
    var imageName =  "error.gif";
	var progress;
    try 
    {
       var progress = new FileProgress(file, this.customSettings.progressTarget);
       progress.setError();
       progress.toggleCancel(false);

       switch (errorCode) {
       case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
        progress.setStatus("Upload Error: " + message);
        this.debug("错误代码: HTTP 错误, 文件名: " + file.name + ", 错误消息: " + message);
        break;
       case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
        progress.setStatus("上传 失败.");
        this.debug("错误代码: 上传失败, 文件名: " + file.name + ", 文件大小: " + file.size + ", 错误消息: " + message);
        break;
       case SWFUpload.UPLOAD_ERROR.IO_ERROR:
        progress.setStatus("服务器 (IO) 错误");
        this.debug("错误代码: IO 错误, 文件名: " + file.name + ", 错误消息: " + message);
        break;
       case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
        progress.setStatus("安全 错误");
        this.debug("错误代码: 安全 错误, 文件名: " + file.name + ", 错误消息: " + message);
        break;
       case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
            imageName = "uploadlimit.gif";
            break;
       case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
        progress.setStatus("检验文件失败，跳过上传.");
        this.debug("错误代码: 检验文件失败，跳过上传, 文件名: " + file.name + ", 文件大小: " + file.size + ", 错误消息: " + message);
        break;
       case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
            try {
				progress = new FileProgress(file,  this.customSettings.upload_target);
				progress.setCancelled();
				progress.setStatus("Cancelled");
				progress.toggleCancel(false);
			}
			catch (ex1) {
				this.debug(ex1);
			}
            break;
       case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
            try {
				progress = new FileProgress(file,  this.customSettings.upload_target);
				progress.setCancelled();
				progress.setStatus("Stopped");
				progress.toggleCancel(true);
			}
			catch (ex2) {
				this.debug(ex2);
			}
            break;
       default:
            progress.setStatus("未知错误: " + errorCode);
            this.debug("错误代码: " + errorCode + ", 文件名: " + file.name + ", 文件大小: " + file.size + ", 错误消息: " + message);
        break;
       }
       addImage("/contents/images/" + imageName);
    } catch (ex) {
            this.debug(ex);
        }
}


function addImage(src) {
	var newImg = document.createElement("img");
	newImg.style.margin = "5px";
    
	document.getElementById("thumbnails").appendChild(newImg);
	if (newImg.filters) {
		try {
			newImg.filters.item("DXImageTransform.Microsoft.Alpha").opacity = 0;
		} catch (e) {
			// If it is not set initially, the browser will throw an error.  This will set it if it is not set yet.
			newImg.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + 0 + ')';
		}
	} else {
		newImg.style.opacity = 0;
	}

	newImg.onload = function () {
		fadeIn(newImg, 0);
	};
	newImg.src = src;
}

function fadeIn(element, opacity) {
	var reduceOpacityBy = 5;
	var rate = 30;	// 15 fps


	if (opacity < 100) {
		opacity += reduceOpacityBy;
		if (opacity > 100) {
			opacity = 100;
		}

		if (element.filters) {
			try {
				element.filters.item("DXImageTransform.Microsoft.Alpha").opacity = opacity;
			} catch (e) {
				// If it is not set initially, the browser will throw an error.  This will set it if it is not set yet.
				element.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + opacity + ')';
			}
		} else {
			element.style.opacity = opacity / 100;
		}
	}

	if (opacity < 100) {
		setTimeout(function () {
			fadeIn(element, opacity);
		}, rate);
	}
}
