// dllmain.cpp : Определяет точку входа для приложения DLL.
//#include "pch.h"

#include <vector>
#include <malloc.h>
#include <cmath>

#include "Structs.h"
#include "Jem.h"

struct SolutionDoubleBuffer {
    bool lastFront = false;
    Array* front = nullptr;
    Array* back = nullptr;
};

bool g_initialized = false;
int g_samplingDensity = 1000;
SolutionDoubleBuffer g_solutions = SolutionDoubleBuffer();

extern "C" {
    _declspec(dllexport) void Initialize(int samplingDensity);
    _declspec(dllexport) void Dispose();
    _declspec(dllexport) Array CreateLsPolynom(const Array& data, int degree);
    _declspec(dllexport) Array DrawGraph(const Array& solution, double from, double to);
    _declspec(dllexport) void SetSamplingDensity(int samplingDensity);
}

Array& GetBuffer();
MatrixMxN CreateCoefficientsMatrix(Point*& points, size_t pointCount, int degree);

void Initialize(int samplingDensity)
{
    if (g_initialized)
        return;

    int length = samplingDensity * 2;
    g_samplingDensity = samplingDensity;
    g_solutions.front = new Array();
    g_solutions.back = new Array();
    g_solutions.front->length = length;
    g_solutions.front->variables = (double*)malloc(sizeof(double) * length);
    g_solutions.back->length = length;
    g_solutions.back->variables = (double*)malloc(sizeof(double) * length);
}

void Dispose()
{
    FreeArray(*g_solutions.front);
    FreeArray(*g_solutions.back);
    delete g_solutions.front;
    delete g_solutions.back;
    g_initialized = false;
}

Array CreateLsPolynom(const Array& data, int degree) {
    size_t pointCount = data.length / 2;
    Point* points = reinterpret_cast<Point*>(data.variables);

    MatrixMxN derivationsMatrix = CreateCoefficientsMatrix(points, pointCount, degree);
    Array solution = SolveMatrix(derivationsMatrix);
    
    FreeMatrix(derivationsMatrix);

    return solution;
}

Array DrawGraph(const Array& solution, double from, double to) {

    Array& buffer = GetBuffer();
    double step = std::ceil(std::abs(to - from)) / g_samplingDensity;
    int length = buffer.length;

    double x = from;
    for (int i = 0; i < length - 1; i += 2, x += step) {
        double y = solution.variables[0];
        double apprValue = x;

        double tmp = apprValue;
        for (int j = 1; j < solution.length; j++, tmp *= apprValue) {
            y += solution.variables[j] * tmp;
        }

        buffer.variables[i] = x;
        buffer.variables[i + 1] = y;
    }

    return buffer;
}

void SetSamplingDensity(int samplingDensity)
{
    if (samplingDensity == g_samplingDensity)
        return;

    Dispose();
    Initialize(samplingDensity);
}

Array& GetBuffer()
{
    bool lastFront = g_solutions.lastFront;
    g_solutions.lastFront = !lastFront;
    if (lastFront)
        return *g_solutions.back;
    else
        return *g_solutions.front;
}

MatrixMxN CreateCoefficientsMatrix(Point*& points, size_t pointCount, int degree) {
    int unknownCount = degree + 1;

    std::vector<double> mainCoefficients = std::vector<double>(unknownCount * 2 - 1);
    std::vector<double> rightSise = std::vector<double>(unknownCount);
    mainCoefficients[0] = (double)pointCount;

    for (int i = 0; i < pointCount; i++) {
        double x = points[i].x;

        double tmp = x;
        for (int j = 1; j < unknownCount * 2 - 1; j++, tmp *= x) {
            mainCoefficients[j] += tmp;
        }

        tmp = 1;
        for (int j = 0; j < unknownCount; j++, tmp *= x) {
            rightSise[j] += points[i].y * tmp;
        }
    }

    double** matrix = (double**)malloc(sizeof(double*) * (unknownCount));
    for (int i = 0; i < unknownCount; i++) {
        matrix[i] = (double*)malloc(sizeof(double) * (unknownCount + 1));
        matrix[0][i] = mainCoefficients[i];
        matrix[i][unknownCount - 1] = mainCoefficients[unknownCount - 1 + i];
        matrix[i][unknownCount] = rightSise[i];
    }

    for (int i = 1; i < unknownCount; i++) {
        for (int j = 0; j < unknownCount - 1; j++) {
            matrix[i][j] = matrix[i - 1][j + 1];
        }
    }

    MatrixMxN result = MatrixMxN();
    result.matrix = matrix;
    result.rowCount = unknownCount;
    result.columnCount = unknownCount + 1;

    return result;
}