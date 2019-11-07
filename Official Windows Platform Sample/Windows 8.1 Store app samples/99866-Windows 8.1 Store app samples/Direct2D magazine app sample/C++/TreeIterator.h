//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

template <typename T>
class TreeIterator
{
public:
    TreeIterator(T^ rootNode)
        : m_currentNode(rootNode)
    {}

    inline T^ GetCurrentNode()
    {
        return m_currentNode;
    }

    size_t operator++()
    {
        if (m_currentNode == nullptr)
        {
            return 0;
        }

        T^ nextNode = m_currentNode->GetFirstChild();

        if (nextNode != nullptr)
        {
            // Go to the first child if exists.
            m_currentNode = nextNode;
        }
        else
        {
            nextNode = m_currentNode->GetNextSibling();

            if (nextNode != nullptr)
            {
                // It's a leaf node, go to the next sibling.
                m_currentNode = nextNode;
            }
            else
            {
                // It's the last leaf node, find a parent with a sibling
                // and go to that sibling.
                T^ node = m_currentNode;

                do
                {
                    node = node->GetParent();

                    if (node == nullptr)
                    {
                        return 0;
                    }

                    nextNode = node->GetNextSibling();

                } while (nextNode == nullptr);

                m_currentNode = nextNode;
            }
        }

        return 1;
    }

private:
    // The node in the tree the iterator is at
    T^ m_currentNode;
};
