// dllmain.cpp : Определяет точку входа для приложения DLL.
//#include "pch.h"

#include <vector>
#include <malloc.h>
#include <cmath>

#include "Structs.h"
#include "Jem.h"

extern "C" {
    _declspec(dllexport) EqualitySolution CreateLskPolynom(double* data, size_t length, int degree);
    _declspec(dllexport) EqualitySolution DrawGraph(const EqualitySolution& solution,
        double from, double to, double step);
}

MatrixMxN CreateCoefficientsMatrix(Point*& points, size_t pointCount, int degree);
inline double ApproximationFunction(double x);

EqualitySolution CreateLskPolynom(double* data, size_t length, int degree) {
    size_t pointCount = length / 2;
    Point* points = reinterpret_cast<Point*>(data);

    MatrixMxN derivationsMatrix = CreateCoefficientsMatrix(points, pointCount, degree);
    EqualitySolution solution = SolveMatrix(derivationsMatrix);

    FreeMatrix(derivationsMatrix);

    return solution;
}

EqualitySolution DrawGraph(const EqualitySolution& solution,
    double from, double to, double step) {
    int length = ((int)std::ceil(std::ceil(std::abs(to - from)) / step)) * 2;

    EqualitySolution graph;
    graph.variables = (double*)malloc(sizeof(double) * length);
    graph.length = length;

    double x = from;
    for (int i = 0; i < length - 1; i += 2, x += step) {
        double y = solution.variables[0];
        double apprValue = ApproximationFunction(x);

        double tmp = apprValue;
        for (int j = 1; j < solution.length; j++, tmp *= apprValue) {
            y += solution.variables[j] * tmp;
        }

        graph.variables[i] = x;
        graph.variables[i + 1] = y;
    }

    return graph;
}

inline double ApproximationFunction(double x)
{
    return x;
}

MatrixMxN CreateCoefficientsMatrix(Point*& points, size_t pointCount, int degree) {
    int unknownCount = degree + 1;

    std::vector<double> mainCoefficients = std::vector<double>(unknownCount * 2 - 1);
    std::vector<double> rightSise = std::vector<double>(unknownCount);
    mainCoefficients[0] = (double)pointCount;

    for (int i = 0; i < pointCount; i++) {
        double x = ApproximationFunction(points[i].x);

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