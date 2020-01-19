#pragma once
#ifdef MYLIBDLL
#define MYLIBDLL extern "C" _declspec(dllexport)
#else
#define MYLIBDLL extern "C" _declspec(dllimport)
#endif // MYLIBDLL

typedef void(CALLBACK *funcCallBack)(int);
typedef void(CALLBACK* ArrayCallBack)(const char*,double*,int&);
MYLIBDLL void RegisterCallBack(funcCallBack);
MYLIBDLL void RegisterArrayCallBack(ArrayCallBack);
MYLIBDLL void Add(int a, int b);
MYLIBDLL void GetArray(char* name);
