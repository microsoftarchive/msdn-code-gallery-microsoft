//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// The thread synchronization events are encapsulated in this 
// class to allow them to easily be passed to the Consumer and 
// Producer classes. 
public class SyncEvents
{
    public SyncEvents()
    {
        // AutoResetEvent is used for the "new item" event because
        // we want this event to reset automatically each time the
        // consumer thread responds to this event.
        _newItemEvent = new AutoResetEvent(false);

        // ManualResetEvent is used for the "exit" event because
        // we want multiple threads to respond when this event is
        // signaled. If we used AutoResetEvent instead, the event
        // object would revert to a non-signaled state with after 
        // a single thread responded, and the other thread would 
        // fail to terminate.
        _exitThreadEvent = new ManualResetEvent(false);

        // The two events are placed in a WaitHandle array as well so
        // that the consumer thread can block on both events using
        // the WaitAny method.
        _eventArray = new WaitHandle[2];
        _eventArray[0] = _newItemEvent;
        _eventArray[1] = _exitThreadEvent;
    }

    // Public properties allow safe access to the events.
    public EventWaitHandle ExitThreadEvent
    {
        get { return _exitThreadEvent; }
    }
    public EventWaitHandle NewItemEvent
    {
        get { return _newItemEvent; }
    }
    public WaitHandle[] EventArray
    {
        get { return _eventArray; }
    }

    private EventWaitHandle _newItemEvent;
    private EventWaitHandle _exitThreadEvent;
    private WaitHandle[] _eventArray;
}

// The Producer class asynchronously (using a worker thread)
// adds items to the queue until there are 20 items.
public class Producer 
{
    public Producer(Queue<int> q, SyncEvents e)
    {
        _queue = q;
        _syncEvents = e;
    }
    public void ThreadRun()
    {
        int count = 0;
        Random r = new Random();
        while (!_syncEvents.ExitThreadEvent.WaitOne(0, false))
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                while (_queue.Count < 20)
                {
                    _queue.Enqueue(r.Next(0, 100));
                    _syncEvents.NewItemEvent.Set();
                    count++;
                }
            }
        }
        Console.WriteLine("Producer thread: produced {0} items", count);
    }
    private Queue<int> _queue;
    private SyncEvents _syncEvents;
}

// The Consumer class uses its own worker thread to consume items
// in the queue. The Producer class notifies the Consumer class
// of new items with the NewItemEvent.
public class Consumer
{
    public Consumer(Queue<int> q, SyncEvents e)
    {
        _queue = q;
        _syncEvents = e;
    }
    public void ThreadRun()
    {
        int count = 0;
        while (WaitHandle.WaitAny(_syncEvents.EventArray) != 1)
        {
            lock (((ICollection)_queue).SyncRoot)
            {
                int item = _queue.Dequeue();
            }
            count++;
        }
        Console.WriteLine("Consumer Thread: consumed {0} items", count);
    }
    private Queue<int> _queue;
    private SyncEvents _syncEvents;
}

public class ThreadSyncSample
{
    private static void ShowQueueContents(Queue<int> q)
    {
        // Enumerating a collection is inherently not thread-safe,
        // so it is imperative that the collection be locked throughout
        // the enumeration to prevent the consumer and producer threads
        // from modifying the contents. (This method is called by the
        // primary thread only.)
        lock (((ICollection)q).SyncRoot)
        {
            foreach (int i in q)
            {
                Console.Write("{0} ", i);
            }
        }
        Console.WriteLine();
    }

    static void Main()
    {
        // Configure struct containing event information required
        // for thread synchronization. 
        SyncEvents syncEvents = new SyncEvents();

        // Generic Queue collection is used to store items to be 
        // produced and consumed. In this case 'int' is used.
        Queue<int> queue = new Queue<int>();

        // Create objects, one to produce items, and one to 
        // consume. The queue and the thread synchronization
        // events are passed to both objects.
        Console.WriteLine("Configuring worker threads...");
        Producer producer = new Producer(queue, syncEvents);
        Consumer consumer = new Consumer(queue, syncEvents);

        // Create the thread objects for producer and consumer
        // objects. This step does not create or launch the
        // actual threads.
        Thread producerThread = new Thread(producer.ThreadRun);
        Thread consumerThread = new Thread(consumer.ThreadRun);

        // Create and launch both threads.     
        Console.WriteLine("Launching producer and consumer threads...");        
        producerThread.Start();
        consumerThread.Start();

        // Let producer and consumer threads run for 10 seconds.
        // Use the primary thread (the thread executing this method)
        // to display the queue contents every 2.5 seconds.
        for (int i = 0; i < 4; i++)
        {
            Thread.Sleep(2500);
            ShowQueueContents(queue);
        }

        // Signal both consumer and producer thread to terminate.
        // Both threads will respond because ExitThreadEvent is a 
        // manual-reset event--so it stays 'set' unless explicitly reset.
        Console.WriteLine("Signaling threads to terminate...");
        syncEvents.ExitThreadEvent.Set();

        // Use Join to block primary thread, first until the producer thread
        // terminates, then until the consumer thread terminates.
        Console.WriteLine("main thread waiting for threads to finish...");
        producerThread.Join();
        consumerThread.Join();
    }
}
