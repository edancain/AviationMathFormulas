# Aviation Math Formulas
A .NET library implementing various aviation mathematical calculations including great circle distance, course calculations, and intersection points.

## Overview
This library provides implementations for:
  - Great Circle Distance Calculations
  - Initial Course Calculations
  - Destination Point Calculations
  - Intersection Point Calculations

## Project Structure
```plaintext

AviationMathFormulas/
├── AviationMathFormulas.Core/      # Core library implementation
│   └── Formulas/
│       ├── CourseCalculator.cs
│       ├── DestinationCalculator.cs
│       ├── DistanceCalculator.cs
│       ├── GeoPoint.cs
│       ├── IntersectionCalculator.cs
│       ├── NavigationConstants.cs
│       └── NavigationUtils.cs
└── AviationMathFormulas.Tests/     # Test project
    └── Tests/
        ├── CourseCalculatorTests.cs
        ├── DestinationCalculatorTests.cs
        ├── DistanceCalculatorTests.cs
        ├── GeoPointTests.cs
        └── InterSectionCalculatorTests.cs
```

## Prerequisites

.NET 8.0 SDK

## Installation

Clone the repository:
```plaintext
git clone https://github.com/edancain/AviationMathFormulas.git
cd AviationMathFormulas
```

## Install the required tools for testing and coverage reporting:
```plaintext
# Install the report generator tool globally
dotnet tool install -g dotnet-reportgenerator-globaltool

# Add .NET tools to your PATH (for zsh users)
cat << \EOF >> ~/.zprofile
# Add .NET Core SDK tools
export PATH="$PATH:$HOME/.dotnet/tools"
EOF

# Reload shell to apply PATH changes
zsh -l
```

## Running Tests
To run the tests:
```plaintext
cd AviationMathFormulas.Tests
dotnet test
```

## Code Coverage
This project uses Coverlet for code coverage and ReportGenerator for coverage reporting.
To generate and view code coverage:

Run tests with coverage collection:
```plaintext
cd AviationMathFormulas.Tests
dotnet test --collect:"XPlat Code Coverage"
```

Generate HTML coverage report:
```plaintext
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

View the coverage report:
```plaintext
open coveragereport/index.html
```

## Understanding Coverage Reports
The coverage report shows:
  - Line Coverage: Percentage of code lines executed during tests
  - Branch Coverage: Percentage of code branches (if/else) tested
  - Method Coverage: Percentage of methods called
  - Class Coverage: Percentage of classes instantiated

## Documentation Links

- Coverlet Documentation: https://github.com/coverlet-coverage/coverlet/blob/master/README.md
- ReportGenerator Documentation: https://reportgenerator.io/
- MSTest Documentation: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-sdk
- .NET Testing Documentation: https://learn.microsoft.com/en-us/dotnet/core/testing/

## Usage Example

```csharp
using AviationMathFormulas.Core.Formulas;

// Calculate distance between two points
var newYork = new GeoPoint(40.7128, -74.0060);
var london = new GeoPoint(51.5074, -0.1278);

// Calculate great circle distance
double distance = DistanceCalculator.CalculateDistance(newYork, london);
double distanceKm = distance * NavigationConstants.EARTH_RADIUS_KM;

// Calculate initial course
double course = CourseCalculator.CalculateInitialCourse(newYork, london);
double courseDegrees = NavigationUtils.ToDegrees(course);

Console.WriteLine($"Distance: {distanceKm:F2} km");
Console.WriteLine($"Initial Course: {courseDegrees:F2}°");
```

## Contributing
I would love to work with anyone that wants to help extend this work. 

## License
This project is licensed under the MIT License - see the LICENSE.md file for details.
