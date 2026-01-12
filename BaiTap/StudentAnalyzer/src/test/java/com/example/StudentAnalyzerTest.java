package com.example;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;
import java.util.Arrays;
import java.util.Collections;

public class StudentAnalyzerTest {
    
    // ===== Test cho phương thức countExcellentStudents =====
    
    @Test
    public void testCountExcellentStudents_NormalCase() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách có nhiều điểm hợp lệ và không hợp lệ: 9.0, 8.5 >= 8.0; 7.0 < 8.0; 11.0, -1.0 không hợp lệ
        assertEquals(2, analyzer.countExcellentStudents(Arrays.asList(9.0, 8.5, 7.0, 11.0, -1.0)));
    }
    
    @Test
    public void testCountExcellentStudents_AllValid() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách toàn bộ hợp lệ và đều >= 8.0
        assertEquals(4, analyzer.countExcellentStudents(Arrays.asList(8.0, 8.5, 9.0, 10.0)));
    }
    
    @Test
    public void testCountExcellentStudents_EmptyList() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách trống
        assertEquals(0, analyzer.countExcellentStudents(Collections.emptyList()));
    }
    
    @Test
    public void testCountExcellentStudents_NullList() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách null
        assertEquals(0, analyzer.countExcellentStudents(null));
    }
    
    @Test
    public void testCountExcellentStudents_BoundaryValues() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách chỉ chứa giá trị 0 và 10
        assertEquals(1, analyzer.countExcellentStudents(Arrays.asList(0.0, 10.0)));
    }
    
    @Test
    public void testCountExcellentStudents_InvalidValues() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Có điểm < 0 hoặc > 10
        assertEquals(0, analyzer.countExcellentStudents(Arrays.asList(-5.0, 11.0, 15.0, -1.0)));
    }
    
    @Test
    public void testCountExcellentStudents_NoExcellent() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Không có học sinh đạt loại Giỏi
        assertEquals(0, analyzer.countExcellentStudents(Arrays.asList(7.0, 7.5, 6.5, 5.0)));
    }
    
    // ===== Test cho phương thức calculateValidAverage =====
    
    @Test
    public void testCalculateValidAverage_NormalCase() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách có nhiều điểm hợp lệ và không hợp lệ
        // Điểm hợp lệ: 9.0, 8.5, 7.0 => trung bình = (9.0 + 8.5 + 7.0) / 3 = 24.5 / 3 ≈ 8.17
        assertEquals(8.17, analyzer.calculateValidAverage(Arrays.asList(9.0, 8.5, 7.0, 11.0, -1.0)), 0.01);
    }
    
    @Test
    public void testCalculateValidAverage_AllValid() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách toàn bộ hợp lệ
        // (8.0 + 9.0 + 10.0) / 3 = 27 / 3 = 9.0
        assertEquals(9.0, analyzer.calculateValidAverage(Arrays.asList(8.0, 9.0, 10.0)), 0.01);
    }
    
    @Test
    public void testCalculateValidAverage_EmptyList() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách trống
        assertEquals(0, analyzer.calculateValidAverage(Collections.emptyList()));
    }
    
    @Test
    public void testCalculateValidAverage_NullList() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách null
        assertEquals(0, analyzer.calculateValidAverage(null));
    }
    
    @Test
    public void testCalculateValidAverage_BoundaryValues() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Danh sách chỉ chứa giá trị 0 và 10
        // (0 + 10) / 2 = 5.0
        assertEquals(5.0, analyzer.calculateValidAverage(Arrays.asList(0.0, 10.0)), 0.01);
    }
    
    @Test
    public void testCalculateValidAverage_AllInvalid() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Tất cả điểm không hợp lệ
        assertEquals(0, analyzer.calculateValidAverage(Arrays.asList(-5.0, 11.0, 15.0, -1.0)), 0.01);
    }
    
    @Test
    public void testCalculateValidAverage_SingleValid() {
        StudentAnalyzer analyzer = new StudentAnalyzer();
        // Chỉ có một điểm hợp lệ
        assertEquals(7.5, analyzer.calculateValidAverage(Arrays.asList(7.5, -1.0, 11.0)), 0.01);
    }
}
