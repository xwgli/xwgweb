using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace XWG.Laboratory.Hubs
{
    /// <summary>
    /// 循环广播器
    /// </summary>
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instance = new Lazy<Broadcaster>(() => new Broadcaster());

        // 服务器向客户端广播消息的时间间隔（毫秒）
        private readonly TimeSpan BroadcastInterval = TimeSpan.FromMilliseconds(1000 / 25);

        private readonly IHubContext _hubContext;

        private Timer _broadcastLoop;

        private ShapeModel _model;

        private bool _modelUpdated;

        public Broadcaster()
        {
            // 获取Hub上下文以方便向连接的客户端发送消息
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<TestHub>();

            _model = new ShapeModel();
            _modelUpdated = false;

            // 开始间隔广播循环
            _broadcastLoop = new Timer(BroadcastShape, null, BroadcastInterval, BroadcastInterval);
        }

        /// <summary>
        /// 向所有客户端广播
        /// </summary>
        /// <param name="state"></param>
        public void BroadcastShape(object state)
        {
            // 仅数据有更新时广播
            if (_modelUpdated)
            {
                // 通过Hub上下文可以在Hub实体外调用其相关方法
                _hubContext.Clients.AllExcept(_model.LastUpdatedBy).updateShape(_model);
                _modelUpdated = false;
            }
        }

        /// <summary>
        /// 更新位置数据
        /// </summary>
        public void UpdateShape(ShapeModel clientModel)
        {
            _model = clientModel;
            _modelUpdated = true;
        }

        /// <summary>
        /// 循环广播器的静态实例
        /// </summary>
        public static Broadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }

    /// <summary>
    /// 测试用集线器
    /// </summary>
    public class TestHub : Hub
    {
        /// <summary>
        /// 存储循环广播器的实例
        /// </summary>
	    private Broadcaster _broadcaster;

        /// <summary>
        /// 初始化集线器
        /// </summary>
	    public TestHub()
	        : this(Broadcaster.Instance)
	    {
	    }

        /// <summary>
        /// 初始化集线器
        /// </summary>
        /// <param name="broadcaster">指定循环广播器</param>
        public TestHub(Broadcaster broadcaster)
	    {
	        _broadcaster = broadcaster;
	    }

        /// <summary>
        /// 更新模型数据
        /// </summary>
        /// <param name="clientModel">模型数据</param>
        public void UpdateModel(ShapeModel clientModel)
        {
            // 获取发送数据的连接Id
            clientModel.LastUpdatedBy = Context.ConnectionId;
            // 使用循环广播器广播数据
            _broadcaster.UpdateShape(clientModel);

            // [否决的]根据连接Id更新除发送者以外的所有浏览器客户端
            // Clients.AllExcept(clientModel.LastUpdatedBy).updateShape(clientModel);
        }
    }

    /// <summary>
    /// 数据模型定义
    /// </summary>
    public class ShapeModel
    {
        // 使用JsonProperty来统一前后台的属性名
        [JsonProperty("left")]
        public double Left { get; set; }

        [JsonProperty("top")]
        public double Top { get; set; }

        // 使用JsonIgnore来在JSON转换中屏蔽掉此属性
        [JsonIgnore]
        public string LastUpdatedBy { get; set; }
    }
}