# Unity-MeshCombine
================================

最近项目上线需要优化性能，因为战斗中场景都是很多小的预设拼出来的，在Unity中勾选了静态合并，但是用显卡绘制工具查看在真机上DC还是比较高，所以就写了下面的工具给美术同学用于合并相同材质的Mesh信息；

> 关于我，欢迎关注  
  微信：[macwink]()

#目录结构

Unity-MeshCombine
|---Assets              
|  |---Artwork 
|  |---MeshCombine 
|  |---MeshCombine
|  |  |---SavedMeshes  // 用于存储合并好的Mesh信息
|  |  |---Scripts // 合并Mesh的脚本文件
|  |  |  |---Editor // 用于Inspector修改，方便美术大爷使用
|  |  |  |---MeshCombine.cs // Mesh合并
|  |  |  |---MaterialDivide.cs // Mesh拆分
|  |---Scene        
|---ProjectSettings      

#如何使用

您只需要将MeshCombine.cs脚本挂载到场景的根节点

