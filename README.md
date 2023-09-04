# Hell C# atx-agent 调用类库

---
###### 初衷只是为了在 C# 中使用 **[UIAutomator2](https://github.com/openatx/uiautomator2)**
---


## 进度
---
> ADBSocket  
> - 主要用于与ADB Server 通讯  
> - 目前只实现了 Serial 方式 通过 HOST 方式连接后续会完善
	
> ADBClient  
> - 对ADBSocket的进一步封装 目的就是为了用得方便

> HAtx (**进行中**)
> - 初始化安装
> - 设备上线
> - RPC (进行中)
---

## 存在的问题
---
> 为什么 C# 原生 HttpWebRequest 和 HttpClient 甚至是 RESTClient 请求接口都特别慢 但是使用 socket 就没有这个问题 有空可以研究一下

## 常用指令存放处
---
> MAC 下删除自动生成的._文件 
> - find . -name "._*"  | xargs rm -f