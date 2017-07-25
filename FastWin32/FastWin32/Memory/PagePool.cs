using System;

namespace FastWin32.Memory
{
    /// <summary>
    /// 内存页面池（用于多线程）
    /// </summary>
    public class PagePool
    {
        private Tuple<IntPtr, long>[] _items;
        private int _length;
        private long _total;
        private bool _isEmpty;
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

                Tuple<IntPtr, long>[] array;

                array = new Tuple<IntPtr, long>[value];
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
        public long Total => _total;

        /// <summary>
        /// 池是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                lock (this)
                {
                    return _isEmpty;
                }
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public Tuple<IntPtr, long> this[int index]
        {
            get
            {
                if (index >= _length)
                    //索引超过有效部分
                    throw new IndexOutOfRangeException();

                return _items[index];
            }
            set
            {
                if (index >= _length)
                    //索引超过有效部分
                    throw new IndexOutOfRangeException();

                _items[index] = value;
            }
        }

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

            _items = new Tuple<IntPtr, long>[capacity];
        }

        /// <summary>
        /// 添加一个内存区域元组
        /// </summary>
        /// <param name="item"></param>
        public void Add(Tuple<IntPtr, long> item)
        {
            if (_length == _items.Length)
                //已满，进行扩容
                Capacity *= 2;

            _total += item.Item2;
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
        /// 从池中取出一个元素
        /// </summary>
        /// <returns></returns>
        public Tuple<IntPtr, long> Pop()
        {
            if (_length == 0)
                //身体被掏空
                throw new IndexOutOfRangeException();

            Tuple<IntPtr, long> item;

            _length--;
            //长度减一
            item = _items[_length];
            _items[_length] = default(Tuple<IntPtr, long>);
            //清除原元素
            _total -= item.Item2;
            //要扫描地址的总数降低
            return item;
        }

        /// <summary>
        /// 返回下一个元素
        /// </summary>
        /// <returns></returns>
        public Tuple<IntPtr, long> Next()
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

        /// <summary>
        /// 移除指定索引的一个元素
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveAt(int index)
        {
            if (index >= _length)
                //索引超过有效部分
                throw new IndexOutOfRangeException();

            _length--;
            _total -= _items[index].Item2;
            if (index < _length)
                Array.Copy(_items, index + 1, _items, index, _length - index);
            _items[_length] = default(Tuple<IntPtr, long>);
        }

        /// <summary>
        /// 返回数组
        /// </summary>
        /// <returns></returns>
        public Tuple<IntPtr, long>[] ToArray()
        {
            Tuple<IntPtr, long>[] array = new Tuple<IntPtr, long>[_length];
            Array.Copy(_items, 0, array, 0, _length);
            return array;
        }
    }
}
