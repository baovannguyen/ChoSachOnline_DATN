import React, { useEffect, useState } from "react";
import axios from "axios";

const apiBase = "/api/RentBook";

export default function RentBooks() {
    const [books, setBooks] = useState([]);
    const [editingBook, setEditingBook] = useState(null);
    const [form, setForm] = useState({
        RentBookId: "",
        Title: "",
        AuthorId: "",
        CategoryId: "",
        Price: 0,
        DuocHienThi: true,
    });

    const fetchBooks = async () => {
        try {
            const res = await axios.get(apiBase);
            setBooks(res.data);
        } catch {
            alert("Lỗi lấy sách thuê");
        }
    };

    useEffect(() => {
        fetchBooks();
    }, []);

    const resetForm = () => {
        setForm({
            RentBookId: "",
            Title: "",
            AuthorId: "",
            CategoryId: "",
            Price: 0,
            DuocHienThi: true,
        });
        setEditingBook(null);
    };

    const onChange = (e) => {
        const { name, value, type, checked } = e.target;
        setForm((f) => ({
            ...f,
            [name]: type === "checkbox" ? checked : value,
        }));
    };

    const onSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingBook) {
                await axios.put(`${apiBase}/${form.RentBookId}`, form);
                alert("Cập nhật thành công");
            } else {
                await axios.post(apiBase, form);
                alert("Thêm sách thuê thành công");
            }
            fetchBooks();
            resetForm();
        } catch {
            alert("Lỗi khi lưu sách thuê");
        }
    };

    const onEdit = (book) => {
        setForm(book);
        setEditingBook(book.RentBookId);
    };

    const onDelete = async (id) => {
        if (!window.confirm("Bạn chắc chắn muốn xóa?")) return;
        try {
            await axios.delete(`${apiBase}/${id}`);
            alert("Xóa thành công");
            fetchBooks();
        } catch {
            alert("Lỗi khi xóa sách thuê");
        }
    };

    const onToggleVisibility = async (id, visible) => {
        try {
            await axios.put(`${apiBase}/hideorshow/${id}?visible=${visible}`);
            fetchBooks();
        } catch {
            alert("Lỗi khi cập nhật trạng thái hiển thị");
        }
    };

    return (
        <div>
            <h2>Quản lý Sách Thuê</h2>
            <form onSubmit={onSubmit}>
                <input
                    name="RentBookId"
                    placeholder="ID sách thuê"
                    value={form.RentBookId}
                    onChange={onChange}
                    disabled={editingBook !== null}
                    required
                />
                <input
                    name="Title"
                    placeholder="Tên sách"
                    value={form.Title}
                    onChange={onChange}
                    required
                />
                <input
                    name="AuthorId"
                    placeholder="ID tác giả"
                    value={form.AuthorId}
                    onChange={onChange}
                    required
                />
                <input
                    name="CategoryId"
                    placeholder="ID thể loại"
                    value={form.CategoryId}
                    onChange={onChange}
                    required
                />
                <input
                    type="number"
                    name="Price"
                    placeholder="Giá thuê"
                    value={form.Price}
                    onChange={onChange}
                    required
                />
                <label>
                    Hiển thị:
                    <input
                        type="checkbox"
                        name="DuocHienThi"
                        checked={form.DuocHienThi}
                        onChange={onChange}
                    />
                </label>
                <button type="submit">{editingBook ? "Cập nhật" : "Thêm"}</button>
                {editingBook && <button onClick={resetForm}>Hủy</button>}
            </form>

            <table border="1" cellPadding={5} style={{ marginTop: 20 }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên sách</th>
                        <th>Tác giả</th>
                        <th>Thể loại</th>
                        <th>Giá thuê</th>
                        <th>Hiển thị</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {books.map((b) => (
                        <tr key={b.RentBookId}>
                            <td>{b.RentBookId}</td>
                            <td>{b.Title}</td>
                            <td>{b.AuthorId}</td>
                            <td>{b.CategoryId}</td>
                            <td>{b.Price}</td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={b.DuocHienThi}
                                    onChange={() => onToggleVisibility(b.RentBookId, !b.DuocHienThi)}
                                />
                            </td>
                            <td>
                                <button onClick={() => onEdit(b)}>Sửa</button>{" "}
                                <button onClick={() => onDelete(b.RentBookId)}>Xóa</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
