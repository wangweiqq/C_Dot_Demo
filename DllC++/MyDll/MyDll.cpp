// MyDll.cpp : 定义 DLL 应用程序的导出函数。
//
#define MYLIBDLL
#include "stdafx.h"
#include "MyDll.h"
#include <functional>
#include <sstream>
#include <iostream>

//std::function<void(int)> func1;
funcCallBack func2 = nullptr;
void RegisterCallBack(funcCallBack callback) {
	func2 = callback;
	//func1 = p;
	//func1 = callback;
}
void Add(int a, int b) {
	int* total = new int;
	*total = a + b;
	//int total = a + b;
	if (func2) {
		func2(*total);
	}
}
ArrayCallBack func3 = nullptr;
void RegisterArrayCallBack(ArrayCallBack callback) {
	func3 = callback;
}
void GetArray(char* name) {
	std::cout << name << std::endl;
	double buff[10];
	for (int i = 0; i < 10; ++i) {
		buff[i] = 0.001*i;
	}
	int size = sizeof(buff) / sizeof(buff[0]);
	std::stringstream strBuf;
	strBuf << "Hello " << size << ", " << name;
	if (func3) {
		func3(strBuf.str().c_str(), buff, size);
	}
}