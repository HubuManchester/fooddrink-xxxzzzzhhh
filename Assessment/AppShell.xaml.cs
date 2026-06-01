﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using Assessment.Views;

namespace Assessment
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(DishDetailPage), typeof(DishDetailPage));
            Routing.RegisterRoute(nameof(CartPage), typeof(CartPage));
            Routing.RegisterRoute(nameof(FullScreenImagePage), typeof(FullScreenImagePage));
        }
    }
}