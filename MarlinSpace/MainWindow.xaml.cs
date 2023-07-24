using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using Galileo6;

namespace MarlinSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /*
         * 4.1	Create two data structures using the LinkedList<T> class from the C# Systems.
         * Collections.Generic namespace. The data must be of type “double”; 
         * you are not permitted to use any additional classes, nodes, 
         * pointers or data structures (array, list, etc) in the implementation of this application.
         * The two LinkedLists of type double are to be declared as global within the “public partial class”.*/
        private const int listSize = 400;
        private bool isSorted = false;
        private const int error_flag = -99;
        private LinkedList<double> sensorAList = new LinkedList<double>();
        private LinkedList<double> sensorBList = new LinkedList<double>();
        //private int sigma = 0;
        //private int MU = 0;
        private const string sigmaFilePath = @"sigmaData.txt";
        private const string muFilePath = @"muData.txt";

        #region DEBUG SETTINGS
        private void initConsoleTraceListener()
        {
            ConsoleTraceListener listener = new ConsoleTraceListener();
            Trace.Listeners.Add(listener);
        }

        #endregion

        #region LOADING DATA
        /**
         4.4	Create a button and associated click method that will call the LoadData and ShowAllSensorData methods.
        The input parameters are empty, and the return type is void.
         */
        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            loadData();
            isSorted = false;
            btnStatusControl();
        }

        private void Marlin_Space_Main_Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this is for debug in the first stage
            initConsoleTraceListener();
            populateComoBox(sigmaFilePath,sigmaInput);
            populateComoBox(muFilePath, MUInput);
            btnStatusControl();
        }

        /**
         4.13	Add two numeric input controls for Sigma and Mu. The value for Sigma must be limited with a minimum of 10
        and a maximum of 20. Set the default value to 10. The value for Mu must be limited with a minimum of 35 and a maximum of 75.
        Set the default value to 50.
         */
        private void populateComoBox(string filePath,ComboBox cb) {
            string[] dataString = System.IO.File.ReadAllLines(filePath);
            for (int i = 0; i < dataString.Length; i++)
            {
                cb.Items.Add(dataString[i]);
            }
        }
        /**
         4.2	Copy the Galileo.DLL file into the root directory of your solution folder and add the appropriate 
        reference in the solution explorer. Create a method called “LoadData” which will populate both LinkedLists.
        Declare an instance of the Galileo library in the method and create the appropriate loop construct to 
        populate the two LinkedList; the data from Sensor A will populate the first LinkedList, 
        while the data from Sensor B will populate the second LinkedList. 
        The LinkedList size will be hardcoded inside the method and must be equal to 400. 
        The input parameters are empty, and the return type is void.
         */
        private void loadData() { 
            sensorAList.Clear();
            sensorBList.Clear();
           
            for(int i = 0; i<listSize;i++)
            {
                sensorAList.AddLast(createNode(sigmaInput.Text,MUInput.Text,0));
                sensorBList.AddLast(createNode(sigmaInput.Text, MUInput.Text,1));
            }
            showAllSensorData(SensorDataListView);

        }

        #endregion

        #region SELECTION SORT
        private void selectionSortBtn_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool isDone = selection_sort(sensorAList);
            sw.Stop();
            double during = sw.Elapsed.TotalMilliseconds;
            
            if (isDone)
            {
                displayListBoxData(sensorAList, sensorAListBox);
                displayPerformanceInMilliseconds(during, selectionSortPerformance);
                isSorted = true;
            }
            else { 
                //pop up error message
            }
            btnStatusControl();
        }

        private void displayPerformanceInMilliseconds(double milliseconds, TextBox  tb) {
            tb.Text = milliseconds.ToString();
        }
        private void displayPerformanceInTicks(long ticks, TextBox tb)
        {
            tb.Text = ticks.ToString();
        }

        /**
         4.7	Create a method called “SelectionSort” which has a single input parameter
        of type LinkedList, while the calling code argument is the linkedlist name. The method code must 
        follow the pseudo code supplied below in the Appendix. The return type is Boolean.
         */
        private bool selection_sort(LinkedList<double> li) {
            int max = NumberOfNodes(li) - 1;
            for (int i = 0; i < max; i++) {
                int min = i;
                for (int j = i; j < max + 1; j++) {
                    if (li.ElementAt(j) < li.ElementAt(min)) {
                        min = j;
                    }
                }

                if (min != i) {
                    if (swapNode(li, min, i) == false)
                    {
                        return false;
                    }
                }
            }
           
            return true;
        }


        #endregion

        /**
         4.12	Create four button click methods that will sort the LinkedList using the Selection and Insertion methods. The four methods are:
        1.	Method for Sensor A and Selection Sort
        2.	Method for Sensor A and Insertion Sort
        3.	Method for Sensor B and Selection Sort
        4.	Method for Sensor B and Insertion Sort
        The button method must start a stopwatch before calling the sort method. Once the sort is complete the stopwatch will stop, and the number of milliseconds will be displayed in a read only textbox. Finally, the code/method will call the “ShowAllSensorData” method and “DisplayListboxData” for the appropriate sensor.
          */
        #region INSERTION SORT

        private void insertSortBtn_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool isDone = insertion_sort(sensorAList);
            sw.Stop();
            double during = sw.Elapsed.TotalMilliseconds;

            if (isDone) {
                displayListBoxData(sensorAList, sensorAListBox);
                displayPerformanceInMilliseconds(during,insertionSortPerformance);
                isSorted = true;
            }
            btnStatusControl();
        }

        /**
         4.8	Create a method called “InsertionSort” which has a single parameter
        of type LinkedList, while the calling code argument is the linkedlist name.
        The method code must follow the pseudo code supplied below in the Appendix. 
        The return type is Boolean.
         */
        private bool insertion_sort(LinkedList<double> li)
        {
            int loop = 0;
            int max = NumberOfNodes(li);
            for (int i = 0; i < max - 1; i++)
            {
                loop++;
                for (int j = i + 1; j > 0; j--)
                {
                    if (li.ElementAt(j - 1) > li.ElementAt(j))
                    {
                        loop++;

                        if (swapNode(li, j - 1, j) == false) return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region Binary Search Iterative


        
        private void BSIterative_Click(object sender, RoutedEventArgs e)
        {
            
            if (isTargetOutofBoundary(sensorAList,transformToDouble(targetBoxA.Text), (sensorAList.Count - 1) / 2))
            {
                ShowOkMessage("The target is out of bounday","Input Error");
            }


            else {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int index = binSearch_Interative(sensorAList, transformToDouble(targetBoxA.Text), sensorAList.Count - 1, 0); 
                sw.Stop();
                long ticks = sw.ElapsedTicks;

                if (index != error_flag)
                {
                    highlightItems(sensorAList, sensorAListBox, index, 2);
                    displayPerformanceInTicks(ticks, interativePerformance);
                }
            }
        }

        /**
         4.9	Create a method called “BinarySearchIterative” which has the following four parameters: 
        LinkedList, SearchValue, Minimum and Maximum. This method will return an integer of the linkedlist element
        from a successful search or the nearest neighbour value. The calling code argument is the linkedlist name, 
        search value, minimum list size and the number of nodes in the list. The method code must follow the pseudo code
        supplied below in the Appendix.
         */
        private int binSearch_Interative(LinkedList<double> li,double target,int maximum,int minimum) { 
            // if (isTargetOutofBoundary(li,target,(maximum+minimum)/2)) return -99 ;
            while (minimum <= maximum - 1)
            {
                int middle = (maximum + minimum) / 2;
                
                if (locateSearchRange(li, middle, target)) {
                   
                    return getClosestIndex(li,middle,target); 
                }
                else if (li.ElementAt(middle) < target) { minimum = middle + 1; }
                else if (li.ElementAt(middle) > target) { maximum = middle - 1; }
            }
            //the value is out of the bundary
            return error_flag;
        }

        //redesign this mathod, 0 means yes and it is the head or the end, 
        //-1 means yes and the gap in the left
        //1 means yes and the gap in the right
        //-99 means no
        private bool locateSearchRange(LinkedList<double> li,int middle,double target) {
           
            if (target > li.ElementAt(middle - 1) && target < li.ElementAt(middle + 1)) {
                
                return true;
            } 
                
            //if not found ,return false
            return false;
        }

        private int getClosestIndex(LinkedList<double> li, int middle, double target) {
            int start = middle - 1;
            int end = middle + 1;
            
            //todo check the boundary
            double valueOfStart = li.ElementAt(start);
            double valueOfMiddle = li.ElementAt(middle);

            double gap1 = Math.Abs(target - valueOfStart);
            
            double gap2 = Math.Abs(valueOfMiddle - target);
           
            double smallest = getSmallerNumber(gap1, gap2);

            if (end < li.Count)
            {
                double valueOfEnd = li.ElementAt(end);
                double gap3 = Math.Abs(valueOfEnd - target);
                smallest = getSmallerNumber(gap3, getSmallerNumber(gap1, gap2));
               
            }
            if (smallest == gap1) { return start; }
            if (smallest == gap2) { return middle; }
           
            return end;
        }

        private bool isIndexOutOfBoundary(int max,int index) {
            if (index < 0 || index > max) { return true; }
            else return false;
        }
        private double getSmallerNumber(double n1, double n2) {
            if (n1 > n2) return n2;
            else return n1;
        }

        #endregion

        /**
         4.11	Create four button click methods that will search the LinkedList for an integer value entered into a textbox on the form. The four methods are:
        1.	Method for Sensor A and Binary Search Iterative
        2.	Method for Sensor A and Binary Search Recursive
        3.	Method for Sensor B and Binary Search Iterative
        4.	Method for Sensor B and Binary Search Recursive
        The search code must check to ensure the data is sorted, then start a stopwatch before calling the search method. Once the search is complete the stopwatch will stop, and the number of ticks will be displayed in a read only textbox. Finally, the code/method will call the “DisplayListboxData” method and highlight the search target number and two values on each side.

         */
        #region BinarySearch Recursive

        private void BSRecursive_Click(object sender, RoutedEventArgs e)
        {

            if (isTargetOutofBoundary(sensorAList, transformToDouble(targetBoxA.Text), (sensorAList.Count - 1) / 2))
            {
                ShowOkMessage("The target is out of bounday", "Input Error");
            }
            else {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int index = binarySearchRecursive(sensorAList, transformToDouble(targetBoxA.Text), 0, sensorAList.Count - 1);
                sw.Stop();
                long ticks = sw.ElapsedTicks;

                if (index != error_flag)
                {
                    displayPerformanceInTicks(ticks, recursivePerformance);
                }
            }
        }

        /**
         4.10	Create a method called “BinarySearchRecursive” which has the following four parameters:
        LinkedList, SearchValue, Minimum and Maximum. This method will return an integer of the linkedlist element
        from a successful search or the nearest neighbour value. The calling code argument is the linkedlist name, 
        search value, minimum list size and the number of nodes in the list. The method code must follow the pseudo code 
        supplied below in the Appendix.
         */
        private int binarySearchRecursive(LinkedList<double> li, double target, int l, int r) {
            //add a loop monitor here
           
            if (r >= l)
            {
                int mid = (l + r) / 2;

                if (locateSearchRange(li, mid, target))
                    return getClosestIndex(li, mid, target);

                // If element is smaller than mid, then
                // it can only be present in left subarray
               
                if (li.ElementAt(mid) > target)
                    return binarySearchRecursive(li, target, l, mid );

                // Else the element can only be present
                else return binarySearchRecursive(li, target, mid , r); 
               
            }

            //if found nothing ,return error flag -99
            return error_flag;
        }
        #endregion


        #region Highlight the listBox

        private void highlightItems(LinkedList<double>li,ListBox lb,int index, int offset) {
            sensorAListBox.SelectedItems.Clear();
            sensorAListBox.SelectedItems.Add(sensorAList.ElementAt(index));
            // hightlight the offset ones if they exists
            for (int i = -(offset); i < offset+1; i++) {
                 if (isIndexOutOfBoundary(li.Count, index +i ) || (index +i) > 0) {
                    lb.SelectedItems.Add(sensorAList.ElementAt(index + i));
                }
            }
        }
        #endregion

        #region UTILS
        private void ShowOkMessage(string message, string title)
        {
            MessageBox.Show(message,title);
           
        }

        /**
         4.5	Create a method called “NumberOfNodes” that will return an integer which is the number of nodes(elements)
        in a LinkedList. 
        The method signature will have an input parameter of type LinkedList, 
        and the calling code argument is the linkedlist name.
         */
        private int NumberOfNodes(LinkedList<double> li)
        {
            return li.Count;
        }

        private double transformToDouble(string value)
        {
            return double.Parse(value);
        }

        private bool isTargetOutofBoundary(LinkedList<double> li, double target, int mid)
        {
            //if the middle = 0 or middle = li.count return true
            if (target > li.ElementAt(li.Count - 1) || target < li.ElementAt(0))
                return true;
            else return false;
        }

        /**
         4.6	Create a method called “DisplayListboxData” that will display the content of a LinkedList inside the appropriate ListBox.
        The method signature will have two input parameters;
        a LinkedList, and the ListBox name.  
        The calling code argument is the linkedlist name and the listbox name.
         */
        private void displayListBoxData(LinkedList<double> li, ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (double data in li)
            {
                listBox.Items.Add(data);
            }
        }

        private bool swapNode(LinkedList<double> li, int x, int y)
        {
            // Nothing to do if x and y are same
            if (x == y)
                return false;
            LinkedListNode<double> currentX = li.Find(li.ElementAt(x));
            LinkedListNode<double> currentY = li.Find(li.ElementAt(y));
            if (currentX == null || currentY == null) return false;

            double temp = currentX.Value;
            currentX.Value = currentY.Value;
            currentY.Value = temp;
            return true;
        }

        private LinkedListNode<double> createNode(string sigma, string mu, int sw)
        {
            Galileo6.ReadData readData = new ReadData();
            LinkedListNode<double> node = new LinkedListNode<double>(0d);
            if (sw == 0)
                node = new LinkedListNode<double>(readData.SensorA(double.Parse(sigma), double.Parse(mu)));
            else if (sw == 1)
                node = new LinkedListNode<double>(readData.SensorB(double.Parse(sigma), double.Parse(mu)));
            return node;
        }

        /**
         4.3	Create a custom method called “ShowAllSensorData” which will display both LinkedLists in a ListView. 
        Add column titles “Sensor A” and “Sensor B” to the ListView. 
        The input parameters are empty, and the return type is void.
         */

        private void showAllSensorData(ListView view)
        {
            view.Items.Clear();
            for (int i = 0; i < listSize; i++)
            {
                view.Items.Add(new
                {
                    SensorA = sensorAList.ElementAt(i),
                    SensorB = sensorBList.ElementAt(i)
                });
            }
        }
        #endregion

        #region INPUT CONTROL
        /**
         4.14	Add two textboxes for the search value; one for each sensor, ensure only numeric integer values can be entered.
         */
        private void targetBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //Regex reg = new Regex(@"^([0-9]{1,4}((\.)[0-9]{0,1})?)$");
            Regex reg = new Regex(@"^([0-9]{1,4}?)$");
            e.Handled = !reg.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
        #endregion

        #region BUTTON STATUS CONTROL

        private void btnStatusControl() {
            if (isSorted == false) {
                BSIterative.IsEnabled = false;
                BSRecursive.IsEnabled = false;
            }
            if (isSorted == true)
            {
                BSIterative.IsEnabled = true;
                BSRecursive.IsEnabled = true;
            }
        }
        #endregion

    }
}
