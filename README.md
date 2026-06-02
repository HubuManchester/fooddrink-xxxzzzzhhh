# Mei Wei Xuan - Cuisine Review and Ordering Application

A cross-platform Chinese cuisine browsing, ordering and reviewing application based on .NET MAUI 8

## Functionality

### Cuisine Browsing

- Gathering **90 **Chinese cuisines, covering **15 cuisines** (Szechuan, Cantonese, Peking, Zhejiang, Shandong, Huaiyang, Shanghai, Northeastern, Hunan, Fujian, Xinjiang). Yunnan, Dim Sum, Noodles, Hotpot, Vegetarian, Dessert, Beverage)
- Page loading, 10 dishes per page, loading animation when scrolling down
- Dish ratings (⭐ ratings), spiciness logo, vegetarian logo, production time and other rich information display
- Dish pictures support double-click to zoom in and out, two-finger zoom (zoom in/zoom out)
- Part of the dishes are equipped with exclusive pictures, the rest of the dishes use the common Placeholder

### Cart

- Add/remove dishes, adjust quantity
- Calculate total price in real time
- Bottom TabBar displays the corner of cart quantity

### Review system

- Star rating for dishes (1-5 stars)
- Write text reviews
- Support taking photos and uploading pictures of dishes
- Display all reviews in reverse chronological order

### Delivery tracking

- GPS positioning to get the current location
- When the authority is insufficient or the positioning fails, the simulated location will be used automatically. Automatically use simulated location (Beijing) when permission is insufficient or location failure occurs 
- Delivery route is displayed on map 

### Interactive

- **Shake**: Turn on the acceleration sensor and shake the phone to randomly recommend dishes 
- Dark/light theme switch 

### Settings

- Personal information editing 
- Dark mode on/off 
- Notification preferences 

## Technical Architecture

### Development Framework

- **.NET 8 + . MAUI**: cross-platform UI framework 
- *CommunityToolkit.Mvvm**: MVVM architecture support (`[ObservableProperty]`, `[RelayCommand]`) 
- *CommunityToolkit.Maui**: MAUI community toolkit 
- *Microsoft.Maui.Controls.Maps**: Maps functionality

### Project structure

```
Assessment/
├── Converters/          # Value converters (boolean, color, rating, etc.)
├── Data/                # Data files (dishes.json - 90 dishes data)
├── Models/              # Data models
│   ├── Dish.cs          # Dish model
│   ├── CartItem.cs      # Cart item model
│   └── Review.cs        # Review model
├── Services/            # Business services layer
│   ├── DishService.cs   # Dish data service (JSON read/write, pagination, sorting)
│   ├── CartService.cs   # Shopping cart service
│   ├── ReviewService.cs # Review service
│   ├── LocationService.cs # Location service (GPS + simulated location fallback)
│   └── ThemeService.cs  # Theme service (dark/light mode switching)
├── ViewModels/          # View models (MVVM)
│   ├── BaseViewModel.cs # Base ViewModel
│   ├── DishListViewModel.cs   # Dish list (pagination, filtering, shake)
│   ├── DishDetailViewModel.cs # Dish details
│   ├── CartViewModel.cs       # Shopping cart
│   ├── ReviewViewModel.cs     # Reviews
│   ├── DeliveryViewModel.cs   # Delivery
│   ├── MapViewModel.cs        # Map
│   ├── SettingsViewModel.cs   # Settings
│   ├── FullScreenImageViewModel.cs # Full-screen image
│   └── WelcomeViewModel.cs    # Welcome page
├── Views/               # XAML pages
│   ├── DishListPage.xaml      # Dish list page
│   ├── DishDetailPage.xaml    # Dish detail page
│   ├── CartPage.xaml          # Cart page
│   ├── ReviewPage.xaml        # Review page
│   ├── DeliveryPage.xaml      # Delivery page
│   ├── MapPage.xaml           # Map page
│   ├── SettingsPage.xaml      # Settings page
│   ├── FullScreenImagePage.xaml # Full-screen image page
│   └── WelcomePage.xaml       # Welcome page
├── Resources/           # Resource files
│   ├── Images/          # Dish images
│   ├── Fonts/           # Fonts
│   ├── Styles/          # Global styles
│   └── AppIcon/         # App icons
├── AppShell.xaml        # App navigation structure
├── MauiProgram.cs       # Dependency injection configuration
└── App.xaml.cs          # App entry point
```

### Dependency Injection

All Services and ViewModels are managed through DI containers: 

- `ThemeService`, `DishService`, `CartService`, `ReviewService`, `LocationService` , 
- are **Singleton**

## Each ViewModel and Page is **Transient**

### Quick Start

- [Environmental Requirements](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET 8 SDK ](https://visualstudio.microsoft.com/)Visual Studio 2022 [ (17.8+) or ](https://code.visualstudio.com/)
- VS Code

### MAUI workload

```bash
dotnet workload install maui
```

### Install MAUI workload

```bash
# clone project
git clone <repository-url>
cd Assessment

# restore dependencies
dotnet restore

# run Android
dotnet build -t:Run -f net8.0-android

# run Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

## Run the project

The dish data is stored in `Data/dishes.json` and contains 90 Chinese delicacies, and each dish contains the following fields:




| Field           | Description                      |
| --------------- | -------------------------------- |
| Id              | Unique identifier                |
| Name            | English name of the dish         |
| Category        | Cuisine category                 |
| Description     | Detailed description of the dish |
| Price           | Price (CNY)                      |
| ImageName       | Image file name                  |
| Rating          | Rating (out of 5.0)              |
| IsSpicy         | Whether the dish is spicy        |
| IsVegetarian    | Whether it is vegetarian         |
| PrepTimeMinutes | Preparation time (minutes)       |

# 