

# AliyunDNS

<!-- PROJECT SHIELDS -->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

### 配置

根目录下生成config.json文件，或直接运行一次：
```json
{
  "V4Url": "https://ipv4.ip.mir6.com",
  "V6Url": "https://ipv6.ip.mir6.com",
  "EndPoint": "https://alidns.cn-hangzhou.aliyuncs.com",
  "Interval": 600,
  "KeyId": "",
  "KeySecret": "",
  "Domains": [
    {
      "DomainName": "",
      "V4Enable": true,
      "V6Enable": true
    }
  ],
  "SubDomains": [
    {
      "DomainName": "",
      "V4Enable": true,
      "V6Enable": true
    }
  ]
}
```
### 文件目录说明

eg:

```
filetree 
├─AliyunDNS      ---控制台程序
│
├─AliyunDNS.Core ---阿里云接口

```

### 使用到的框架

- [serilog](https://github.com/serilog/serilog)

### 版权说明

该项目签署了MIT 授权许可，详情请参阅 [LICENSE.txt](https://github.com/fallingrust/AliyunDNS/LICENSE.txt)

<!-- links -->

[your-project-path]:fallingrust/AliyunDNS

[contributors-shield]: https://img.shields.io/github/contributors/fallingrust/AliyunDNS.svg?style=flat-square

[contributors-url]: https://github.com/fallingrust/AliyunDNS/graphs/contributors

[forks-shield]: https://img.shields.io/github/forks/fallingrust/AliyunDNS.svg?style=flat-square

[forks-url]: https://github.com/fallingrust/AliyunDNS/network/members

[stars-shield]: https://img.shields.io/github/stars/fallingrust/AliyunDNS.svg?style=flat-square

[stars-url]: https://github.com/fallingrust/AliyunDNS/stargazers

[issues-shield]: https://img.shields.io/github/issues/fallingrust/AliyunDNS.svg?style=flat-square

[issues-url]: https://img.shields.io/github/issues/fallingrust/AliyunDNS.svg

[license-shield]: https://img.shields.io/github/license/fallingrust/AliyunDNS.svg?style=flat-square

[license-url]: https://github.com/fallingrust/AliyunDNS/blob/master/LICENSE.txt
