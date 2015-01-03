using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Pub.Class {
    /// <summary>
    /// HeapSorter 堆排序 O(n*log2n)~O(n*log2n) | O(1) 
    /// 
    /// 修改纪录
    ///     2010.01.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>    
    public static class HeapSorter<T> where T : System.IComparable {
        #region Sort
        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isAsc"></param>
        public static void Sort(IList<T> list, bool isAsc) {
            //如果是升序，将堆调整成一个最大堆，如果是降序，调整成最小堆
            for (int i = list.Count / 2 - 1; i >= 0; i--) {
                HeapSorter<T>.Adjust(list, i, list.Count - 1, isAsc);
            }
            //把跟结点跟最后一个结点的值交换，然后把除最后结点以外的结点调整成堆
            for (int i = list.Count - 1; i > 0; i--) {
                list.Swap<T>(0, i);
                HeapSorter<T>.Adjust(list, 0, i - 1, isAsc);
            }
        }
        #endregion

        #region Adjust
        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="nodeIndx"></param>
        /// <param name="maxAdjustIndx"></param>
        /// <param name="isAsc"></param>
        private static void Adjust(IList<T> list, int nodeIndx, int maxAdjustIndx, bool isAsc) {
            T rootValue = list[nodeIndx];
            T temp = list[nodeIndx];
            int childNodeIndx = 2 * nodeIndx + 1;
            while (childNodeIndx <= maxAdjustIndx) {
                if (isAsc) {
                    if (childNodeIndx < maxAdjustIndx && list[childNodeIndx].CompareTo(list[childNodeIndx + 1]) < 0) {
                        childNodeIndx++;
                    }
                    if (rootValue.CompareTo(list[childNodeIndx]) > 0) {
                        break;
                    } else {
                        list[(childNodeIndx - 1) / 2] = list[childNodeIndx];
                        childNodeIndx = 2 * childNodeIndx + 1;
                    }
                } else {
                    if (childNodeIndx < maxAdjustIndx && list[childNodeIndx].CompareTo(list[childNodeIndx + 1]) > 0) {
                        childNodeIndx++;
                    }
                    if (rootValue.CompareTo(list[childNodeIndx]) < 0) {
                        break;
                    } else {
                        list[(childNodeIndx - 1) / 2] = list[childNodeIndx];
                        childNodeIndx = 2 * childNodeIndx + 1;
                    }
                }

            }
            list[(childNodeIndx - 1) / 2] = temp;
        }
        #endregion
    }
    /// <summary>
    /// InsertionSorter 插入排序 O(n2) | O(1) 
    /// 
    /// 修改纪录
    ///     2010.01.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class InsertionSorter<T> where T : System.IComparable {
        #region Sort
        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isAsc"></param>
        public static void Sort(IList<T> list, bool isAsc) {
            T next;
            int j;
            for (int i = 1; i < list.Count; i++) {
                next = list[i];
                if (isAsc) {
                    for (j = i - 1; j >= 0 && list[j].CompareTo(next) > 0; j--) {
                        list[j + 1] = list[j];
                    }
                } else {
                    for (j = i - 1; j >= 0 && list[j].CompareTo(next) < 0; j--) {
                        list[j + 1] = list[j];
                    }
                }
                list[j + 1] = next;
            }
        }
        #endregion
    }
    /// <summary>
    /// MergeSorter 归并排序
    /// 
    /// 修改纪录
    ///     2010.01.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>    
    public static class MergeSorter<T> where T : System.IComparable {
        #region Sort
        /// <summary>
        /// 归并排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isAsc"></param>
        public static void Sort(IList<T> list, bool isAsc) {
            int length = 1;
            IList<T> sortedList = new List<T>(list.Count);
            foreach (T item in list) {
                sortedList.Add(item);
            }
            while (length < list.Count) {
                MergeSorter<T>.MergePass(list, sortedList, length, isAsc);
                length *= 2;
                MergeSorter<T>.MergePass(sortedList, list, length, isAsc);
                length *= 2;
            }
        }
        #endregion

        #region private
        private static void MergePass(IList<T> list, IList<T> sortedList, int length, bool isASC) {
            int i, j;
            for (i = 0; i <= list.Count - 2 * length; i += 2 * length) {
                MergeSorter<T>.Merge(list, i, i + length - 1, i + 2 * length - 1, sortedList, isASC);
            }
            if (i + length < list.Count) {
                MergeSorter<T>.Merge(list, i, i + length - 1, list.Count - 1, sortedList, isASC);
            } else {
                for (j = i; j < list.Count; j++) {
                    sortedList[j] = list[j];
                }
            }

        }
        /// <summary>
        /// 归并两个已经排序的子序列为一个有序的序列
        /// </summary>
        /// <param name="list">原始序列</param>
        /// <param name="startIndx">归并起始index</param>
        /// <param name="splitIndx">第一段结束的index</param>
        /// <param name="endIndx">第二段结束的index</param>
        /// <param name="sortedList">排序后的序列</param>
        /// <param name="isASC">是否升序</param>
        private static void Merge(IList<T> list, int startIndx, int splitIndx, int endIndx, IList<T> sortedList, bool isASC) {
            int right = splitIndx + 1;
            int left = startIndx;
            int sortIndx = startIndx;
            while (left <= splitIndx && right <= endIndx) {
                if (isASC) {
                    if (list[left].CompareTo(list[right]) <= 0) {
                        sortedList[sortIndx++] = list[left++];
                    } else {
                        sortedList[sortIndx++] = list[right++];
                    }
                } else {
                    if (list[left].CompareTo(list[right]) >= 0) {
                        sortedList[sortIndx++] = list[left++];
                    } else {
                        sortedList[sortIndx++] = list[right++];
                    }
                }
            }
            if (left > splitIndx) {
                for (int t = right; t <= endIndx; t++) {
                    sortedList[sortIndx + t - right] = list[t];
                }
            } else {
                for (int t = left; t <= splitIndx; t++) {
                    sortedList[sortIndx + t - left] = list[t];
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// QuickSorter 快速排序 O(n*log2n)~O(n2) | O(log2n)~O(n) 
    /// 
    /// 修改纪录
    ///     2010.01.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class QuickSorter<T> where T : System.IComparable {
        /// <summary>
        /// 快速排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isAsc"></param>
        public static void Sort(IList<T> list, bool isAsc) {
            QuickSorter<T>.Sort(list, 0, list.Count - 1, isAsc);
        }

        #region Sort
        private static void Sort(IList<T> list, int left, int right, bool isAsc) {
            int i, j;
            T pivot;
            if (left < right) {
                i = left;
                j = right + 1;
                if (isAsc) {
                    while (i < j) {
                        i++;
                        pivot = list[left];
                        while (i < right && list[i].CompareTo(pivot) < 0) {
                            i++;
                        }
                        j--;
                        while (j >= left && list[j].CompareTo(pivot) > 0) {
                            j--;
                        }
                        if (i < j) {
                            list.Swap<T>(i, j);
                        }
                    }
                } else {
                    while (i < j) {
                        i++;
                        pivot = list[left];
                        while (i < right && list[i].CompareTo(pivot) > 0) {
                            i++;
                        }
                        j--;
                        while (j >= left && list[j].CompareTo(pivot) < 0) {
                            j--;
                        }
                        if (i < j) {
                            list.Swap<T>(i, j);
                        }
                    }

                }
                list.Swap<T>(left, j);
                QuickSorter<T>.Sort(list, left, j - 1, isAsc);
                QuickSorter<T>.Sort(list, j + 1, right, isAsc);
            }
        }
        #endregion
    }
    //二叉树排序 O(n*log2n)~O(n2) | O(n)
    //希尔排序 O | O(1)
    //快速排序>归并排序>堆排序>希尔排序
}
