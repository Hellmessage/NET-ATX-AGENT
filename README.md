# Hell C# atx-agent 调用类库

---
###### 初衷只是为了在 C# 中使用 **[UIAutomator2](https://github.com/openatx/uiautomator2)**
---


## 进度
---
> ADBSocket  
> - 主要用于与ADB Server 通讯  
> - Usb(Serial) (已完成)
> - WIFI Host (未进行)
	
> ADBClient  
> - 对ADBSocket的进一步封装 目的就是为了用得方便

> HSocket
> - Http 请求
> - WebSocket (未进行) 

> HAtx (**进行中**)
> - 初始化安装
>	- Initer 
> - 设备上线
>	- RunUiautomator
> - UINode[Selector] (UI选择器)
>   - Click
>   - Text
>   - Exists
>	- **待进行[更复杂的多级查询]**
> - XPATH (简单实现 可能有BUG)
> - RPC (进行中)
>	- Click
>	- DoubleClick
>	- LongClick
>	- Press
>	- Swipe
>	- Drag
>	- ScreenOn
>	- ScreenOff
>	- WaitForExists
>	- FreezeRotation
>	- SetOrientation
>	- GetWindowSize
>	- GetOrientation
>	- DeviceInfo
>	- Info
>   - Touch(Down, Move, Up)
---

## 存在的问题
---
> 为什么 C# 原生 HttpWebRequest 和 HttpClient 甚至是 RESTClient 请求接口都特别慢 但是使用 socket 就没有这个问题 有空可以研究一下

## 常用指令存放处
---
> MAC 下删除自动生成的._文件 
> - find . -name "._*"  | xargs rm -f