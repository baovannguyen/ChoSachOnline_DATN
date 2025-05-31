// App.jsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';

import Categories from './admin/Categories';
import Authors from './admin/Authors';
import Books from './admin/Books';
import RentBooks from './admin/RentBooks';

function App() {
    return (
        <Router>
            <nav style={{ marginBottom: '20px' }}>
                <Link to="/categories" style={{ marginRight: 10 }}>Thể loại</Link>
                <Link to="/authors" style={{ marginRight: 10 }}>Tác giả</Link>
                <Link to="/books" style={{ marginRight: 10 }}>Sách bán</Link>
                <Link to="/rentbooks">Sách thuê</Link>
            </nav>

            <Routes>
                <Route path="/" element={<Categories />} />
                <Route path="/categories" element={<Categories />} />
                <Route path="/authors" element={<Authors />} />
                <Route path="/books" element={<Books />} />
                <Route path="/rentbooks" element={<RentBooks />} />
            </Routes>
        </Router>
    );
}

export default App;