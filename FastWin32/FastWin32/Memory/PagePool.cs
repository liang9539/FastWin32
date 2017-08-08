using System;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存页面池（用于多线程）
    /// </summary>
    internal class PagePool
    {
        private Tuple<IntPtr, IntPtr>[] _items;
        private int _length;
        private IntPtr _total;
        private bool _isEmpty;
        private bool _is64Bit;
        private int _current;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _length)
                    //指定的新容量大小小于内部集合有数据部分长度
                    throw new ArgumentException();

                Tuple<IntPtr, IntPtr>[] array;

                array = new Tuple<IntPtr, IntPtr>[value];
                //将_items转移到这里
                Array.Copy(_items, 0, array, 0, _length);
                //将有数据部分转移
                _items = array;
            }
        }

        /// <summary>
        /// 内存区域总数
        /// </summary>
        public int Count => _length;

        /// <summary>
        /// 内存区域总数
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// 要搜寻的内存大小
        /// </summary>
        public IntPtr Total => _total;

        /// <summary>
        /// 池是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                lock (this)
                    return _isEmpty;
            }
        }

        /// <summary>
        /// 指示目标进程是否为64位进程
        /// </summary>
        public bool Is64Bit => _is64Bit;

        /// <summary>
        /// 实例化（默认容量200）
        /// </summary>
        public PagePool() : this(200) { }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public PagePool(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException();

            _items = new Tuple<IntPtr, IntPtr>[capacity];
        }

        /// <summary>
        /// 添加一个内存页面元组
        /// </summary>
        /// <param name="item"></param>
        public void Add(Tuple<IntPtr, IntPtr> item)
        {
            if (_length == _items.Length)
                //已满，进行扩容
                Capacity *= 2;

            if (_is64Bit)
                _total = (IntPtr)((long)_total + (long)item.Item2);
            else
                _total = (IntPtr)((int)_total + (int)item.Item2);
            _items[_length] = item;
            _length++;
        }

        /// <summary>
        /// 清空池
        /// </summary>
        public void Clear()
        {
            if (_length > 0)
            {
                //集合大小大于0
                Array.Clear(_items, 0, _length);
                _length = 0;
            }
        }

        /// <summary>
        /// 返回下一个元素
        /// </summary>
        /// <returns></returns>
        public Tuple<IntPtr, IntPtr> Next()
        {
            lock (this)
            {
                if (_isEmpty)
                    return null;

                try
                {
                    return _items[_current];
                }
                finally
                {
                    _current++;
                    if (_current == _length)
                        //到尽头了
                        _isEmpty = true;
                }
            }
        }
    }
}
